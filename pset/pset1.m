%% Q3
clear; clc; close all;
s = tf('s');
H_full = 1 / ((s+5)*(s+6)*(s+100));
H_approx = 0.01 / ((s+5)*(s+6));
figure(1);
subplot(2,1,1); step(H_full, H_approx); title('Step Response H(s)'); legend('Full', 'Approx');
subplot(2,1,2); bode(H_full, H_approx); title('Bode Plot H(s)'); legend('Full', 'Approx');

G_full = 1 / ((s+5)*(s+6)*(s+7));
G_approx = (1/7) / ((s+5)*(s+6));
figure(2);
subplot(2,1,1); step(G_full, G_approx); title('Step Response G(s)'); legend('Full', 'Approx');
subplot(2,1,2); bode(G_full, G_approx); title('Bode Plot G(s)'); legend('Full', 'Approx');

P_full = 1 / ((s+15)*(s^2+0.5*s+1));
P_approx = (1/15) / (s^2+0.5*s+1);
figure(3);
subplot(2,1,1); step(P_full, P_approx); title('Step Response P(s)'); legend('Full', 'Approx');
subplot(2,1,2); bode(P_full, P_approx); title('Bode Plot P(s)'); legend('Full', 'Approx');
%% Q5
% Clean up previous runs
clc; clear; close all;

% 1. Define the Plant Transfer Function P(s)
% P(s) = 120 / (s^2 + 40s + 50)
s = tf('s');
P = 120 / (s^2 + 40*s + 50);

% Define Controller Gains to iterate through
Kp_values = [2, 10, 50];

% --- Figure 1: Step Response Setup ---
fig1 = figure(1); 
hold on; grid on;

% --- Figure 2: Loop Gain Bode Setup ---
fig2 = figure(2); 
hold on; grid on;

% --- Figure 3: Closed Loop Bode Setup ---
fig3 = figure(3); 
hold on; grid on;

% Loop through each Kp value
for i = 1:length(Kp_values)
    Kp = Kp_values(i);
    
    % Define Systems
    C = Kp;                 % Proportional Controller
    L = C * P;              % Loop Gain (for Phase Margin)
    G_cl = feedback(L, 1);  % Closed-Loop System
    
    % --- Plotting ---
    
    % 1. Step Response (Figure 1)
    set(0, 'CurrentFigure', fig1); % Select Figure 1
    step(G_cl);
    
    % 2. Loop Gain Bode (Figure 2)
    set(0, 'CurrentFigure', fig2); % Select Figure 2
    h = bodeplot(L);
    p = getoptions(h); % Get default options
    p.PhaseMatching = 'on'; % Ensure phase doesn't wrap weirdly
    setoptions(h, p);
    
    % 3. Closed Loop Bode (Figure 3)
    set(0, 'CurrentFigure', fig3); % Select Figure 3
    bode(G_cl);
    
    % --- Calculations for Console Output ---
    [Gm, Pm, Wcg, Wcp] = margin(L); 
    bw = bandwidth(G_cl);           
    
    fprintf('Kp = %2d | Phase Margin: %5.1f deg (at %5.1f rad/s) | Bandwidth: %6.1f rad/s\n', ...
        Kp, Pm, Wcp, bw);
en

% --- Apply Descriptive Titles & Legends AFTER plotting ---

% Figure 1 Customization
figure(1);
title({'Step Response of Closed-Loop System G_{cl}(s)'});
legend('K_p = 2 (Overdamped)', 'K_p = 10 (Underdamped)', 'K_p = 50 (High Overshoot)', 'Location', 'SouthEast');
ylabel('Amplitude (x_a)');
xlabel('Time (seconds)');

% Figure 2 Customization
figure(2);
title({'Bode Plot of Loop Gain L(s)'});
legend('K_p = 2', 'K_p = 10', 'K_p = 50', 'Location', 'SouthWest');

% Figure 3 Customization
figure(3);
title({'Bode Plot of Closed-Loop System G_{cl}(s)'});
legend('K_p = 2', 'K_p = 10', 'K_p = 50', 'Location', 'SouthWest');
%% Q6

%% Q7
clear; clc; close all;

s = tf('s');
Kp = 50;
Ki = 500;
Kd = 0.1;
wf = 1e6;

P = 1000 / (s * (s + 1000));

C = Kp + Ki/s + (Kd*s*wf)/(s + wf);

L = C * P;
Gcl = feedback(L, 1);
figure(Name="System Frequency Response");
hold on;
bode(P, 'b');
bode(C, 'g');
bode(L, 'r');
bode(Gcl, 'k');
grid on;
legend('Plant P(s)', 'Controller C(s)', 'Loop Gain L(s)', 'Closed-Loop Gcl(s)');
title('Bode Plot');
hold off;

%% Q7.6 - Part 6 verification (margin + bandwidth)
clear; clc; close all;

s  = tf('s');
wf = 1e6;

% Plant
P = 1000/(s*(s+1000));

% ---- Your tuned gains (example that meets PM≈90°, BW≈1500 rad/s)
Kp = 1500;
Ki = 15;
Kd = 1.5;

% Controller (filtered derivative)
C = Kp + Ki/s + Kd*s*(wf/(s + wf));

% Loop gain and closed-loop
L = C*P;
Gcl = feedback(L, 1);   % = L/(1+L)

% ---- Verification numbers
[GM, PM, Wcg, Wcp] = margin(L);
BW = bandwidth(Gcl);

fprintf('Verification results:\n');
fprintf('  Kp = %.6f, Ki = %.6f, Kd = %.6f, wf = %.2e rad/s\n', Kp, Ki, Kd, wf);
fprintf('  Phase margin PM = %.3f deg at w_gc = %.3f rad/s\n', PM, Wcg);
fprintf('  Closed-loop bandwidth = %.3f rad/s\n', BW);

% Optional: show plots used for proof
figure; margin(L); grid on; title('Open-loop L(s) margins');
figure; bode(Gcl); grid on; title('Closed-loop G_{cl}(s) bode');

%% Q8
clear; clc; close all;
Kp = 10;
Ts = 0.008;
Ps = tf(15, [1, 3]);

disp('Discrete Plant P(z):');
Pz = c2d(Ps, Ts, 'zoh')


figure(1);
bode(Ps, Pz);
legend('P(s) - Continuous', 'P(z) - Discrete')
title('Bode Plot of Continuous vs Discrete')
grid on

Cz = tf(Kp, 1, Ts);
Lz = series(Cz, Pz);
Gcl_z = feedback(Lz, 1);
figure(2);
bode(Pz, Cz, Lz, Gcl_z);
legend('P(z)', 'C(z)', 'L(z)', 'G_{cl}(z)')
title('Bode Plot Comparison')
grid on

%% Q9
clear; clc; close all;
Ts = 5e-3;
Ps = tf(300, [1 255 0]);
disp('Q9 Discrete Plant:')
Pz = c2d(Ps, Ts, 'zoh')

figure(1);
bode(Ps, Pz);
legend('P(s) - Continuous', 'P(z) - Discrete');
title('Bode Plot Comparison')
grid on;

figure(2)
margin(Pz)
title('Bode with Kp = 1')
grid on;
[mag, phase, w] = bode(Pz);
phase = squeeze(phase);
mag = squeeze(mag);
[min_diff, idx] = min(abs(phase - (-120)));
target_mag = mag(idx);
Kp_calculated = 1 / target_mag;
fprintf('Frequency at -120 deg phase: %.2f rad/s\n', w(idx));
fprintf('Magnitude of P(z) at the freq we calculated: %.4f\n', target_mag);
fprintf('Kp = 1/(G(wa)) = %.4f\n', Kp_calculated);

Kp = Kp_calculated;
Cz = tf(Kp, 1, Ts);
Lz = series(Cz, Pz);
Gcl_z = feedback(Lz, 1);

figure(3);
bode(Pz, Cz, Lz, Gcl_z);
legend('P(z)', 'C(z)', 'L(z)', 'G_{cl}(z)')
title('Tuned with Kp')
grid on;

[gm, pm, wcg, wcp] = margin(Lz);
fprintf('Verified phase margin: %.2f degrees \n', pm);

%% Q10
clear; clc; close all;
R = 5;
J = 0.05;
B = 5;
Kt = 0.4;
Ke_emf = 0.2;
V_supply = 12;
K_enc_rev = 1000;
K_sensor = K_enc_rev / (2* pi);

denom = (R*B) + (Kt * Ke_emf);
K_motor = Kt / denom;
tau_motor = R * J / denom;

fprintf('K = %.5f, Tau = %.5f s \n', K_motor, tau_motor);

s = tf('s');
P_velocity = K_motor / (tau_motor * s + 1);
P_position = P_velocity * (1/s);
P_total = V_supply * P_position * K_sensor;

Target_BW_Rad = 100 * 2 * pi;

opts = pidtuneOptions(PhaseMargin = 80, ...
                       DesignFocus = 'reference-tracking');

[C_pid, info] = pidtune(P_total, 'pidf', Target_BW_Rad, opts);
% From docs, I found that pidf includes the filter as well
info
Kp = C_pid.Kp;
Ki = C_pid.Ki;
Kd = C_pid.Kd;
Tf = C_pid.Tf;
Wf = 1/Tf;

disp('------------------------------------------------');
disp('FINAL CONTROLLER PARAMETERS:');
fprintf('Kp: %.4f\n', Kp);
fprintf('Ki: %.4f\n', Ki);
fprintf('Kd: %.4f\n', Kd);
fprintf('Filter Cutoff (wf): %.2e rad/s (Tf = %.2e s)\n', Wf, Tf);
disp('------------------------------------------------');

fprintf('Achieved Crossover Freq: %.2f Hz\n', info.CrossoverFrequency/(2*pi));
fprintf('Achieved Phase Margin: %.2f degrees\n', info.PhaseMargin);

% --- 5. Plots (Requirement 2 & 3) ---

% Define Loop Gain L(s) and Closed Loop Gcl(s)
L_s = C_pid * P_total;
G_cl = feedback(L_s, 1);

% Plot 1: Bode Plots
figure('Name', 'Bode Response');
bode(P_total, 'b', C_pid, 'g', L_s, 'r', G_cl, 'k--');
grid on;
legend('Plant P(s)', 'Controller C(s)', 'Open Loop L(s)', 'Closed Loop G_{cl}(s)');
title('Bode Diagram (Simplified Model + Filtered PID)');

% Plot 2: Step Response
figure('Name', 'Step Response');
step(G_cl);
grid on;
title('Closed-Loop Step Response');
ylabel('Position (Encoder Counts)');

% Verify Bandwidth
bw = bandwidth(G_cl);
fprintf('Actual Closed-Loop Bandwidth: %.2f Hz\n', bw/(2*pi));

%% Q11
clear; clc; close all;
R = 5;
J = 0.05;
B = 5;
Kt = 0.4;
Ke_emf = 0.2;
V_supply = 12;
K_enc_rev = 1000;
K_sensor = K_enc_rev / (2* pi);

Ts = 1e-3;

denom = (R*B) + (Kt * Ke_emf);
K_motor = Kt / denom;
tau_motor = R * J / denom;

fprintf('K = %.5f, Tau = %.5f s \n', K_motor, tau_motor);

%continuous domain
s = tf('s');
P_velocity = K_motor / (tau_motor * s + 1);
P_position = P_velocity * (1/s);
P_continuous = V_supply * P_position * K_sensor;

Target_BW_Rad = 100 * 2 * pi;
opts = pidtuneOptions('PhaseMargin', 80, 'DesignFocus', 'reference-tracking');
[C_continuous, ~] = pidtune(P_continuous, 'pidf', Target_BW_Rad, opts);

fprintf('Continuous PID (C(s)): Kp=%.4f, Ki=%.4f, Kd=%.4f, Tf=%.5f\n', ...
        C_continuous.Kp, C_continuous.Ki, C_continuous.Kd, C_continuous.Tf);

% convert to digital
P_discrete = c2d(P_continuous, Ts, 'zoh') % Based on our graph, this
% should be using ZOH
C_discrete = c2d(C_continuous, Ts, 'tustin')
L_discrete = C_discrete * P_discrete
G_cl_discrete = feedback(L_discrete, 1)

figure('Name', 'Digital Control Analysis');
bode(P_continuous, 'b'); hold on;
bode(C_continuous, 'g');
bode(C_continuous * P_continuous, 'r'); % Continuous Open Loop
% Add Discrete plots (Note: Bode for discrete signals is valid up to Nyquist freq)
h = bodeplot(P_discrete, C_discrete, L_discrete, G_cl_discrete);
setoptions(h, 'FreqUnits', 'Hz', 'PhaseMatching', 'on');
legend('P(s)', 'C(s)', 'L(s)', 'P(z)', 'C(z)', 'L(z)', 'G_{cl}(z)');
title('Continuous vs Discrete Frequency Response');
grid on;

[Gm_c, Pm_c, Wcg_c, Wcp_c] = margin(C_continuous * P_continuous);
[Gm_d, Pm_d, Wcg_d, Wcp_d] = margin(L_discrete);

fprintf('\n--- Phase Margin Comparison ---\n');
fprintf('Continuous Phase Margin: %.2f deg at %.1f Hz\n', Pm_c, Wcp_c/(2*pi));
fprintf('Discrete Phase Margin:   %.2f deg at %.1f Hz\n', Pm_d, Wcp_d/(2*pi));

% --- 4. Difference Equation ---
C_discrete

[num, den] = tfdata(C_discrete, 'v');

fprintf('\n--- Difference Equation Coefficients ---\n');
disp('Discrete Controller C(z) coefficients:');
disp(['Numerator (b0, b1, b2): ', num2str(num)]);
disp(['Denominator (1, a1, a2): ', num2str(den)]);

fprintf('\nFINAL DIFFERENCE EQUATION:\n');
fprintf('u[k] = %.4f*u[k-1] + %.4f*u[k-2] + ...\n', -den(2), -den(3));
fprintf('       %.4f*e[k] + %.4f*e[k-1] + %.4f*e[k-2]\n', num(1), num(2), num(3));