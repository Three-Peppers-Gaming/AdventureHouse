using System;
using System.Timers;
using Terminal.Gui;

namespace AdventureRealms.Services.AdventureClient.UI.TerminalGui
{
    /// <summary>
    /// Manages focus for the Terminal.Gui Adventure Client
    /// Ensures the input field maintains focus for user commands
    /// </summary>
    public class TerminalGuiFocusManager : IDisposable
    {
        private readonly TextField _inputField;
        private readonly FrameView _inputView;
        
        // Focus tracking fields
        private bool _inputFieldShouldHaveFocus = true;
        private bool _isSettingFocusProgrammatically = false;
        private System.Timers.Timer? _focusMonitorTimer;

        public TerminalGuiFocusManager(TextField inputField, FrameView inputView)
        {
            _inputField = inputField ?? throw new ArgumentNullException(nameof(inputField));
            _inputView = inputView ?? throw new ArgumentNullException(nameof(inputView));
        }

        /// <summary>
        /// Start the focus monitoring system
        /// </summary>
        public void StartFocusMonitor()
        {
            _focusMonitorTimer = new System.Timers.Timer(100); // Check every 100ms
            _focusMonitorTimer.Elapsed += OnFocusMonitorTick;
            _focusMonitorTimer.Start();
        }

        /// <summary>
        /// Stop the focus monitoring system
        /// </summary>
        public void StopFocusMonitor()
        {
            _focusMonitorTimer?.Stop();
            _focusMonitorTimer?.Dispose();
            _focusMonitorTimer = null;
        }

        /// <summary>
        /// Set whether the input field should have focus
        /// </summary>
        public void SetInputFieldShouldHaveFocus(bool shouldHaveFocus)
        {
            _inputFieldShouldHaveFocus = shouldHaveFocus;
            if (shouldHaveFocus)
            {
                EnsureInputFieldHasFocus();
            }
        }

        /// <summary>
        /// Ensure the input field has focus using robust management
        /// </summary>
        public void EnsureInputFieldHasFocus()
        {
            if (!_inputFieldShouldHaveFocus) return;
            
            try
            {
                _isSettingFocusProgrammatically = true;
                
                // First make sure the input field is focusable
                if (!_inputField.CanFocus)
                {
                    _inputField.CanFocus = true;
                }
                
                // Set focus multiple ways to ensure it works
                if (_inputField.HasFocus == false)
                {
                    _inputField.SetFocus();
                    
                    // Also try setting it as the focused control on the parent
                    if (_inputView != null)
                    {
                        _inputView.SetFocus();
                        _inputField.SetFocus();
                    }
                }
                
                // Ensure title is correct
                if (_inputView.Title.ToString() != "Command")
                {
                    _inputView.Title = "Command";
                }
            }
            finally
            {
                _isSettingFocusProgrammatically = false;
            }
        }

        /// <summary>
        /// Handle input field gaining focus
        /// </summary>
        public void OnInputFieldEnter()
        {
            if (_isSettingFocusProgrammatically) return;
            
            // When command input gets focus, restore the original frame title
            _inputView.Title = "Command";
            _inputFieldShouldHaveFocus = true;
        }

        /// <summary>
        /// Handle input field losing focus
        /// </summary>
        public void OnInputFieldLeave()
        {
            if (_isSettingFocusProgrammatically) return;
            
            // When leaving command input, update the frame title to show focus instruction
            _inputView.Title = "Command <= Click here to set focus";
            _inputFieldShouldHaveFocus = false;
        }

        /// <summary>
        /// Handle mouse clicks on input-related views
        /// </summary>
        public void OnInputViewClicked()
        {
            _inputFieldShouldHaveFocus = true;
            EnsureInputFieldHasFocus();
        }

        private void OnFocusMonitorTick(object? sender, ElapsedEventArgs e)
        {
            // Run focus check on main thread
            Application.MainLoop.Invoke(() =>
            {
                if (_inputFieldShouldHaveFocus && !_inputField.HasFocus && !_isSettingFocusProgrammatically)
                {
                    // Focus was lost, try to recover it
                    EnsureInputFieldHasFocus();
                }
            });
        }

        public void Dispose()
        {
            StopFocusMonitor();
        }
    }
}