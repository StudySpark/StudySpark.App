using System;

namespace StudySpark.GUI.WPF.Core {
    public class LoginViewEventArgs : EventArgs {
        public enum LoginViewEvent {
            USERDATASUBMITTED,
            LOGINSUCCESS,
            LOGINFAILED
        }

        public LoginViewEvent LoginViewEventType { get; set; }
    }
}
