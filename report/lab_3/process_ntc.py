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


def plot_run(times: List[float], temps: List[float], tau: float,
             t63: float, file: Path, t_initial: float, t_final: float,
             figure_dir: Path) -> str:
    time_arr = np.array(times)
    temp_arr = np.array(temps)
    fit = first_order_fit(time_arr, t_initial, t_final, tau, times[0])

    fig, ax = plt.subplots(figsize=(6.2, 3.4))
    ax.plot(time_arr, temp_arr, label='Measured', color='#1f77b4', linewidth=1.4)
    if fit is not None:
        ax.plot(time_arr, fit, '--', color='#d62728',
                label=f'First-order fit ($\\tau={tau:0.1f}$ s)')
    if t63 is not None:
        ax.axvline(t63, color='#2ca02c', linestyle=':', linewidth=1.2,
                   label=f'$t_{{63\\%}}={t63:0.1f}$ s')
    ax.set_xlabel('Time [s]')
    ax.set_ylabel('Temperature [°C]')
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
    ax.set_xlabel('Time constant τ [s]')
    ax.set_yticks(positions)
    ax.set_yticklabels(labels)
    ax.grid(axis='x', alpha=0.3)
    fig.tight_layout()

    fig_path = figure_dir / 'ntc_tau_summary.png'
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

        n = len(times)
        start_n = max(5, n // 40)
        end_n = max(5, n // 40)
        t_initial = sum(temps[:start_n]) / start_n
        t_final = sum(temps[-end_n:]) / end_n
        tau, log_ts, ratios = compute_tau(times, temps, times[0], t_initial, t_final)
        t63 = estimate_t63(times, temps, t_initial, t_final)
        last_samples = temps[-min(50, n):]
        noise = statistics.pstdev(last_samples) if len(last_samples) > 1 else 0.0

        figure_name = plot_run(times, temps, tau, t63, file, t_initial, t_final, FIG_DIR)
        figure_manifest.append({'file': file.name, 'figure': figure_name})

        results.append({
            'file': file.name,
            'transition': classify_transition(file.name),
            'start_temp': t_initial,
            'end_temp': t_final,
            'tau': tau,
            't63': t63,
            'noise': noise,
            'duration': times[-1] - times[0],
            'samples': n,
        })

    results.sort(key=lambda item: item['file'])

    with SUMMARY_PATH.open('w', encoding='utf-8') as fh:
        json.dump(results, fh, indent=2)

    summary_rows = group_statistics(results)
    with GROUP_PATH.open('w', encoding='utf-8') as fh:
        json.dump(summary_rows, fh, indent=2)

    tau_summary_fig = plot_tau_summary(summary_rows, FIG_DIR)

    print(f'Processed {len(results)} runs.')
    print(f'Per-run summary -> {SUMMARY_PATH}')
    print(f'Group statistics -> {GROUP_PATH}')
    print(f'Individual figures saved in {FIG_DIR}')
    if tau_summary_fig:
        print(f'Tau summary figure: {tau_summary_fig}')


if __name__ == '__main__':
    main()
