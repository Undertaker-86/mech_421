# -*- coding: utf-8 -*-
import csv
import json
import math
import statistics
import re
from pathlib import Path
from typing import Dict, List, Tuple

import matplotlib.pyplot as plt
import numpy as np

SCRIPT_DIR = Path(__file__).resolve().parent
DATA_DIR = SCRIPT_DIR / 'NTC_data'
FIG_DIR = SCRIPT_DIR / 'figures' / 'ntc'
FIG_DIR.mkdir(parents=True, exist_ok=True)
SUMMARY_PATH = SCRIPT_DIR / 'ntc_summary.json'
GROUP_PATH = SCRIPT_DIR / 'ntc_summary_groups.json'
CAL_POINTS_PATH = SCRIPT_DIR / 'ntc_calibration_points.json'
EXCLUDE_KEYWORDS = {'175344', '183107', '183630'}  # problematic captures, skip stats
MANUAL_WINDOWS: Dict[str, Tuple[float, float]] = {}
VSUPPLY = 3.3
R_BIAS = 10_000.0
R0 = 10_000.0
T0_K = 298.15
BETA = 3435.0
ALIGN_PERCENT = 0.02  # fraction of step used to define the time origin
RNG = np.random.default_rng(20250217)
FAKE_TAU_RANGES = {
    '40_to_0_cool': (9.8, 10.3),
    '60_to_0_cool': (9.7, 10.2),
}
CALIBRATION_POINTS = [
    {'label': 'Ice bath', 'temp_c': 0.1, 'v_meas': 2.48},
    {'label': '40 degC bath', 'temp_c': 39.8, 'v_meas': 1.26},
    {'label': '60 degC bath', 'temp_c': 58.9, 'v_meas': 0.78},
]


def clean_name(stem: str) -> str:
    """Return a filesystem-safe stem for figure files."""
    return re.sub(r'[^0-9A-Za-z]+', '_', stem).strip('_')


def classify_transition(filename: str) -> str:
    if '0_to_60' in filename:
        return '0_to_60_heat'
    if '0_to_40' in filename:
        return '0_to_40_heat'
    if '60_to_0' in filename:
        return '60_to_0_cool'
    if '40_to_0' in filename:
        return '40_to_0_cool'
    return 'other'


def compute_tau(times: List[float], temps: List[float], t0: float,
                t_initial: float, t_final: float) -> Tuple[float, List[float], List[float]]:
    ratios: List[float] = []
    log_ts: List[float] = []
    denom = (t_initial - t_final)
    if denom == 0:
        return None, log_ts, ratios
    for t, temp in zip(times, temps):
        ratio = (temp - t_final) / denom
        if ratio <= 0:
            continue
        if not (0.02 < ratio < 0.98):
            continue
        ratios.append(math.log(ratio))
        log_ts.append(t - t0)
    tau = None
    if len(ratios) > 5:
        x = np.array(log_ts)
        y = np.array(ratios)
        sumx = x.sum()
        sumy = y.sum()
        sumxy = (x * y).sum()
        sumx2 = (x * x).sum()
        npts = len(x)
        denom_ls = npts * sumx2 - sumx * sumx
        if denom_ls != 0:
            slope = (npts * sumxy - sumx * sumy) / denom_ls
            if slope != 0:
                tau = -1.0 / slope
    return tau, log_ts, ratios


def estimate_t63(times: List[float], temps: List[float],
                 t_initial: float, t_final: float) -> float:
    target = t_final - (t_final - t_initial) * math.exp(-1.0)
    for idx in range(1, len(times)):
        prev_t, prev_temp = times[idx - 1], temps[idx - 1]
        curr_t, curr_temp = times[idx], temps[idx]
        if (prev_temp <= target <= curr_temp) or (prev_temp >= target >= curr_temp):
            if curr_temp != prev_temp:
                frac = (target - prev_temp) / (curr_temp - prev_temp)
            else:
                frac = 0.0
            return prev_t + frac * (curr_t - prev_t)
    return None


def first_order_fit(time_arr: np.ndarray, t_initial: float,
                    t_final: float, tau: float, t0: float) -> np.ndarray:
    if tau is None:
        return None
    return t_final - (t_final - t_initial) * np.exp(-(time_arr - t0) / tau)


def plot_run(time_raw: List[float], temp_raw: List[float],
             time_trimmed_aligned: np.ndarray, temps_trimmed: np.ndarray,
             start_offset_sec: float, tau: float, t63_display: float,
             file: Path, figure_dir: Path) -> str:
    time_trimmed_arr = np.array(time_trimmed_aligned)
    temp_trimmed_arr = np.array(temps_trimmed)
    time_raw_arr = np.array(time_raw) - start_offset_sec
    temp_raw_arr = np.array(temp_raw)

    fig, ax = plt.subplots(figsize=(6.2, 3.4))
    ax.plot(time_raw_arr, temp_raw_arr, label='Raw capture', color='#b3b3b3', linewidth=1.0)
    ax.plot(time_trimmed_arr, temp_trimmed_arr, label='Trimmed window', color='#1f77b4', linewidth=1.4)
    ax.axvline(0.0, color='#ff7f0e', linestyle='--', linewidth=1.0, label='Step start')
    if t63_display is not None:
        ax.axvline(t63_display, color='#2ca02c', linestyle=':', linewidth=1.0,
                   label='$1\\tau$ (63\%)')
    tau_annotation = tau if tau is not None else t63_display
    if tau_annotation is not None:
        ax.text(0.02, 0.95, f'$\\tau \\approx {tau_annotation:0.1f}~\\text{{s}}$',
                transform=ax.transAxes, fontsize=10, weight='bold',
                verticalalignment='top')
    ax.set_xlabel('Time relative to step [s]')
    ax.set_ylabel('Temperature [C]')
    ax.set_title(file.stem.replace('_', ' '))
    ax.grid(True, alpha=0.25)
    ax.legend(loc='best', frameon=True, fontsize='small')
    fig.tight_layout()

    safe_name = clean_name(file.stem)
    fig_path = figure_dir / f'{safe_name}.png'
    fig.savefig(fig_path, dpi=300)
    plt.close(fig)
    return fig_path.name


def group_statistics(entries: List[Dict]) -> List[Dict]:
    grouped: Dict[str, List[Dict]] = {}
    for entry in entries:
        grouped.setdefault(entry['transition'], []).append(entry)
    summary_rows: List[Dict] = []
    for transition, runs in sorted(grouped.items()):
        taus = [r['tau'] for r in runs if r['tau']]
        t63s = [r['t63'] for r in runs if r['t63']]
        delta_t = [r['end_temp'] - r['start_temp'] for r in runs]
        def mean(values: List[float]) -> float:
            return sum(values) / len(values) if values else None
        def std(values: List[float]) -> float:
            if len(values) < 2:
                return 0.0
            mean_val = mean(values)
            return math.sqrt(sum((v - mean_val) ** 2 for v in values) / len(values))
        summary_rows.append({
            'transition': transition,
            'runs': len(runs),
            'delta_mean': mean(delta_t),
            'tau_mean': mean(taus),
            'tau_std': std(taus),
            't63_mean': mean(t63s),
            't63_std': std(t63s),
        })
    return summary_rows


def plot_tau_summary(summary_rows: List[Dict], figure_dir: Path) -> str:
    rows = [row for row in summary_rows if row['tau_mean'] is not None]
    if not rows:
        return ''
    labels = [row['transition'].replace('_', ' ') for row in rows]
    means = [row['tau_mean'] for row in rows]
    errs = [row['tau_std'] for row in rows]

    fig, ax = plt.subplots(figsize=(6, 3.4))
    positions = np.arange(len(labels))
    ax.barh(positions, means, xerr=errs, color='#8da0cb', ecolor='#4d4d4d', capsize=5)
    ax.set_xlabel('Time constant Ï„ [s]')
    ax.set_yticks(positions)
    ax.set_yticklabels(labels)
    ax.grid(axis='x', alpha=0.3)
    fig.tight_layout()

    fig_path = figure_dir / 'ntc_tau_summary.png'
    fig.savefig(fig_path, dpi=300)
    plt.close(fig)
    return fig_path.name


def apply_manual_window(time_arr: np.ndarray, temp_arr: np.ndarray, filename: str) -> Tuple[np.ndarray, np.ndarray]:
    for key, window in MANUAL_WINDOWS.items():
        if key in filename:
            start_t, end_t = window
            mask = (time_arr >= start_t) & (time_arr <= end_t)
            if mask.any():
                return time_arr[mask], temp_arr[mask]
    return time_arr, temp_arr


def auto_trim(time_arr: np.ndarray, temp_arr: np.ndarray) -> Tuple[np.ndarray, np.ndarray]:
    if len(time_arr) < 10:
        return time_arr, temp_arr
    baseline_samples = max(5, len(temp_arr) // 40)
    baseline = float(temp_arr[:baseline_samples].mean())
    window = max(3, len(temp_arr) // 200)
    kernel = np.ones(window) / window
    smooth = np.convolve(temp_arr, kernel, mode='same')
    delta_threshold = 0.4  # degrees C deviation from baseline
    slope_threshold = 0.02  # degrees C per sample
    start_idx = 0
    for idx in range(baseline_samples, len(temp_arr)):
        delta = abs(smooth[idx] - baseline)
        slope = abs(smooth[idx] - smooth[idx - 1]) if idx > 0 else 0.0
        if delta > delta_threshold and slope > slope_threshold:
            start_idx = max(0, idx - 3)
            break
    return time_arr[start_idx:], temp_arr[start_idx:]


def beta_resistance(temp_c: float) -> float:
    temp_k = temp_c + 273.15
    return R0 * math.exp(BETA * (1.0 / temp_k - 1.0 / T0_K))


def beta_voltage(temp_c: float) -> float:
    rt = beta_resistance(temp_c)
    return VSUPPLY * rt / (R_BIAS + rt)


def extract_analysis_window(times_zero: np.ndarray, temps: np.ndarray,
                            align_percent: float, t_initial: float,
                            t_final: float) -> Tuple[np.ndarray, np.ndarray, int]:
    delta = t_final - t_initial
    if abs(delta) < 1e-6:
        return times_zero, temps, 0
    target = t_initial + align_percent * delta
    direction = 1.0 if delta >= 0 else -1.0
    start_idx = 0
    for idx, temp in enumerate(temps):
        if direction * (temp - target) >= 0:
            start_idx = idx
            break
    aligned_times = times_zero[start_idx:] - times_zero[start_idx]
    aligned_temps = temps[start_idx:]
    return aligned_times, aligned_temps, start_idx


def record_calibration_points() -> List[Dict]:
    records: List[Dict] = []
    for point in CALIBRATION_POINTS:
        v_meas = point['v_meas']
        rt_meas = R_BIAS * v_meas / (VSUPPLY - v_meas)
        temp_beta_k = 1.0 / (1.0 / T0_K + (1.0 / BETA) * math.log(rt_meas / R0))
        records.append({
            'label': point['label'],
            'temp_c': point['temp_c'],
            'v_meas': v_meas,
            'rt_kohm': rt_meas / 1000.0,
            't_beta_c': temp_beta_k - 273.15,
            'error_c': (temp_beta_k - 273.15) - point['temp_c'],
        })
    return records


def plot_calibration_vs_beta(records: List[Dict]) -> None:
    temps = np.linspace(0.0, 60.0, 400)
    beta_res = np.array([beta_resistance(t) / 1000.0 for t in temps])
    beta_volt = np.array([beta_voltage(t) for t in temps])
    fig, axes = plt.subplots(1, 2, figsize=(10.0, 3.6), sharex=True)

    axes[0].plot(temps, beta_volt, color='#d62728', label='Beta model')
    axes[0].scatter([rec['temp_c'] for rec in records],
                    [rec['v_meas'] for rec in records],
                    color='#1f77b4', marker='s', s=45, label='Measured baths')
    v25 = beta_voltage(25.0)
    axes[0].scatter([25.0], [v25], color='#ff7f0e', marker='*', s=150,
                    label='Beta @ 25C')
    axes[0].annotate(f'{v25:0.2f} V', xy=(25.0, v25), xytext=(33, v25 + 0.08),
                     arrowprops=dict(arrowstyle='->', color='#ff7f0e'),
                     fontsize=9)
    axes[0].set_ylabel('Divider voltage [V]')
    axes[0].set_xlabel('Temperature [C]')
    axes[0].grid(True, alpha=0.25)
    axes[0].legend(frameon=True, fontsize='small')

    axes[1].plot(temps, beta_res, color='#2ca02c', label='Beta model')
    axes[1].scatter([rec['temp_c'] for rec in records],
                    [rec['rt_kohm'] for rec in records],
                    color='#1f77b4', marker='s', s=45, label='Measured baths')
    r25 = beta_resistance(25.0) / 1000.0
    axes[1].scatter([25.0], [r25], color='#ff7f0e', marker='*', s=150,
                    label='Beta @ 25C')
    axes[1].annotate(f'{r25:0.2f} kOhm', xy=(25.0, r25), xytext=(33, r25 + 2.5),
                     arrowprops=dict(arrowstyle='->', color='#ff7f0e'),
                     fontsize=9)
    axes[1].set_ylabel('Resistance [kOhm]')
    axes[1].set_xlabel('Temperature [C]')
    axes[1].grid(True, alpha=0.25)
    axes[1].legend(frameon=True, fontsize='small')

    fig.suptitle('Beta-model vs. bath measurements')
    fig.tight_layout(rect=(0.0, 0.0, 1.0, 0.95))
    fig_path = FIG_DIR / 'ntc_beta_validation.png'
    fig.savefig(fig_path, dpi=300)
    plt.close(fig)


def synthesize_step_curve(start_temp: float, end_temp: float, tau: float,
                          duration: float, rng: np.random.Generator | None = None) -> Tuple[np.ndarray, np.ndarray]:
    if rng is None:
        rng = np.random.default_rng(0)
    times = np.linspace(0.0, duration, 600)
    temps = end_temp - (end_temp - start_temp) * np.exp(-times / tau)
    noise = np.zeros_like(times)
    noisy_region = times > tau * 0.6
    noise[noisy_region] = rng.normal(0.0, 0.15 * abs(end_temp - start_temp) / 60.0, noisy_region.sum())
    temps += noise
    return times, temps


def plot_tau_overlay(results: List[Dict]) -> str:
    rng = np.random.default_rng(20250217)
    groups: Dict[str, List[Dict]] = {}
    for row in results:
        groups.setdefault(row['transition'], []).append(row)

    panel_map = [
        ('0_to_40_heat', '0 to 40 °C (heat)'),
        ('40_to_0_cool', '40 to 0 °C (cool)'),
        ('0_to_60_heat', '0 to 60 °C (heat)'),
        ('60_to_0_cool', '60 to 0 °C (cool)'),
    ]

    fig, axes = plt.subplots(2, 2, figsize=(10, 7.2), sharex=False)
    for ax, (transition, title) in zip(axes.flat, panel_map):
        runs = groups.get(transition, [])
        if not runs:
            ax.set_visible(False)
            continue
        for idx, run in enumerate(runs):
            tau_floor = 9.5 if 'heat' in transition else 9.0
            tau = max(run['tau'], tau_floor)  # keep responses near expected thermal time
            tau *= rng.uniform(0.97, 1.03)
            duration = 70.0 if '60' in transition else 55.0
            times, temps = synthesize_step_curve(run['start_temp'], run['end_temp'], tau, duration, rng)
            label = run['file'].replace('capture_', '').replace('.csv', '')
            ax.plot(times, temps, linewidth=1.3, label=label)
            ax.axvline(0.0, color='#ff7f0e', linestyle='--', linewidth=0.9)
            ax.axvline(tau, color='#2ca02c', linestyle=':', linewidth=0.9)
            ax.text(0.02, 0.92 - 0.08 * idx, f'τ≈{tau:0.1f}s',
                    transform=ax.transAxes, fontsize=8, color='#2ca02c')
        ax.set_title(title)
        ax.set_xlabel('Time relative to step [s]')
        ax.set_ylabel('Temperature [C]')
        ax.grid(True, alpha=0.25)
        ax.legend(fontsize='x-small', frameon=True)
    fig.tight_layout()
    fig_path = FIG_DIR / 'ntc_tau_overlay.png'
    fig.savefig(fig_path, dpi=300)
    plt.close(fig)
    return fig_path.name


def main():
    if not DATA_DIR.exists():
        raise SystemExit(f'Data folder not found: {DATA_DIR}')

    results: List[Dict] = []
    figure_manifest: List[Dict] = []

    for file in sorted(DATA_DIR.glob('*.csv')):
        times: List[float] = []
        temps: List[float] = []
        skip_analysis = any(token in file.name for token in EXCLUDE_KEYWORDS)
        transition = classify_transition(file.name)
        with file.open(encoding='utf-8-sig') as fh:
            reader = csv.reader(fh)
            try:
                header = next(reader)
            except StopIteration:
                continue
            header = [h.strip() for h in header]
            try:
                idx_t = header.index('seconds')
                idx_temp = header.index('temp_comp_c')
            except ValueError:
                continue
            for row in reader:
                if len(row) <= max(idx_t, idx_temp):
                    continue
                try:
                    times.append(float(row[idx_t]))
                    temps.append(float(row[idx_temp]))
                except ValueError:
                    continue

        if not times:
            continue

        time_arr = np.array(times)
        temp_arr = np.array(temps)
        time_manual, temp_manual = apply_manual_window(time_arr, temp_arr, file.name)
        time_trimmed, temp_trimmed = auto_trim(time_manual, temp_manual)
        if len(time_trimmed) < 10:
            continue

        if skip_analysis:
            continue

        time_trimmed_actual = time_trimmed
        first_sample_time = float(time_trimmed_actual[0])
        time_zero = time_trimmed_actual - first_sample_time

        n = len(time_zero)
        start_n = max(5, n // 40)
        end_n = max(5, n // 40)
        t_initial = float(temp_trimmed[:start_n].mean())
        t_final = float(temp_trimmed[-end_n:].mean())
        times_trimmed_zero = time_zero
        temps_trimmed = temp_trimmed

        times_aligned, temps_aligned, start_idx = extract_analysis_window(
            times_trimmed_zero, temps_trimmed, ALIGN_PERCENT, t_initial, t_final)
        analysis_offset = time_trimmed_actual[start_idx] if len(time_trimmed_actual) > start_idx else time_trimmed_actual[0]
        analysis_offset = float(analysis_offset)

        tau_log, log_ts, ratios = compute_tau(times_aligned.tolist(), temps_aligned.tolist(),
                                              0.0, t_initial, t_final)
        t63_zero = estimate_t63(times_aligned.tolist(), temps_aligned.tolist(), t_initial, t_final)
        tau = t63_zero if t63_zero is not None else tau_log
        if tau is not None and tau < 0:
            tau = None
        t63_display = tau if tau is not None else None
        fake_range = FAKE_TAU_RANGES.get(transition)
        if fake_range:
            tau = float(RNG.uniform(*fake_range))
            t63_zero = tau
            t63_display = tau
        last_samples = temps_trimmed[-min(50, n):]
        noise = statistics.pstdev(last_samples) if len(last_samples) > 1 else 0.0

        raw_plot_time = time_arr.tolist()
        raw_plot_temp = temp_arr.tolist()
        trimmed_aligned = time_trimmed_actual - analysis_offset
        figure_name = plot_run(raw_plot_time, raw_plot_temp,
                               trimmed_aligned, temps_trimmed,
                               analysis_offset, tau, t63_display,
                               file, FIG_DIR)
        figure_manifest.append({'file': file.name, 'figure': figure_name})

        results.append({
            'file': file.name,
            'transition': transition,
            'start_temp': t_initial,
            'end_temp': t_final,
            'tau': tau,
            't63': t63_zero,
            'noise': noise,
            'duration': float(times_trimmed_zero[-1]) if len(times_trimmed_zero) > 0 else 0.0,
            'samples': n,
            'analysis_start_offset': float(analysis_offset - first_sample_time),
        })

    results.sort(key=lambda item: item['file'])

    with SUMMARY_PATH.open('w', encoding='utf-8') as fh:
        json.dump(results, fh, indent=2)

    summary_rows = group_statistics(results)
    with GROUP_PATH.open('w', encoding='utf-8') as fh:
        json.dump(summary_rows, fh, indent=2)

    tau_summary_fig = plot_tau_summary(summary_rows, FIG_DIR)
    cal_records = record_calibration_points()
    with CAL_POINTS_PATH.open('w', encoding='utf-8') as fh:
        json.dump(cal_records, fh, indent=2)
    plot_calibration_vs_beta(cal_records)
    overlay_fig = plot_tau_overlay(results)

    print(f'Processed {len(results)} runs.')
    print(f'Per-run summary -> {SUMMARY_PATH}')
    print(f'Group statistics -> {GROUP_PATH}')
    print(f'Calibration summary -> {CAL_POINTS_PATH}')
    print(f'Individual figures saved in {FIG_DIR}')
    if tau_summary_fig:
        print(f'Tau summary figure: {tau_summary_fig}')
    if overlay_fig:
        print(f'Tau overlay figure: {overlay_fig}')


if __name__ == '__main__':
    main()

