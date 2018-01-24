﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.261.
// 
#pragma warning disable 1591

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins.localhost1 {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceSoap", Namespace="http://tempuri.org/")]
    public partial class Service : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback VersionOperationCompleted;
        
        private System.Threading.SendOrPostCallback ExecuteOperationCompleted;
        
        private System.Threading.SendOrPostCallback BatchExecuteOperationCompleted;
        
        private System.Threading.SendOrPostCallback SingleExecuteOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Service() {
            this.Url = global::CambridgeSoft.COE.Registration.Services.RegistrationAddins.Properties.Settings.Default.CambridgeSoft_COE_Registration_RegistrationAddins_localhost1_Service;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event VersionCompletedEventHandler VersionCompleted;
        
        /// <remarks/>
        public event ExecuteCompletedEventHandler ExecuteCompleted;
        
        /// <remarks/>
        public event BatchExecuteCompletedEventHandler BatchExecuteCompleted;
        
        /// <remarks/>
        public event SingleExecuteCompletedEventHandler SingleExecuteCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Version", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Version() {
            object[] results = this.Invoke("Version", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void VersionAsync() {
            this.VersionAsync(null);
        }
        
        /// <remarks/>
        public void VersionAsync(object userState) {
            if ((this.VersionOperationCompleted == null)) {
                this.VersionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnVersionOperationCompleted);
            }
            this.InvokeAsync("Version", new object[0], this.VersionOperationCompleted, userState);
        }
        
        private void OnVersionOperationCompleted(object arg) {
            if ((this.VersionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.VersionCompleted(this, new VersionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Execute", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool Execute(string code, string returnName, out object retval, out string error) {
            object[] results = this.Invoke("Execute", new object[] {
                        code,
                        returnName});
            retval = ((object)(results[1]));
            error = ((string)(results[2]));
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void ExecuteAsync(string code, string returnName) {
            this.ExecuteAsync(code, returnName, null);
        }
        
        /// <remarks/>
        public void ExecuteAsync(string code, string returnName, object userState) {
            if ((this.ExecuteOperationCompleted == null)) {
                this.ExecuteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnExecuteOperationCompleted);
            }
            this.InvokeAsync("Execute", new object[] {
                        code,
                        returnName}, this.ExecuteOperationCompleted, userState);
        }
        
        private void OnExecuteOperationCompleted(object arg) {
            if ((this.ExecuteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ExecuteCompleted(this, new ExecuteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/BatchExecute", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int BatchExecute(string code, string[] inputVars, string[] inputs, string[] outputVars, out object[] outputs, out string[] errors) {
            object[] results = this.Invoke("BatchExecute", new object[] {
                        code,
                        inputVars,
                        inputs,
                        outputVars});
            outputs = ((object[])(results[1]));
            errors = ((string[])(results[2]));
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void BatchExecuteAsync(string code, string[] inputVars, string[] inputs, string[] outputVars) {
            this.BatchExecuteAsync(code, inputVars, inputs, outputVars, null);
        }
        
        /// <remarks/>
        public void BatchExecuteAsync(string code, string[] inputVars, string[] inputs, string[] outputVars, object userState) {
            if ((this.BatchExecuteOperationCompleted == null)) {
                this.BatchExecuteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnBatchExecuteOperationCompleted);
            }
            this.InvokeAsync("BatchExecute", new object[] {
                        code,
                        inputVars,
                        inputs,
                        outputVars}, this.BatchExecuteOperationCompleted, userState);
        }
        
        private void OnBatchExecuteOperationCompleted(object arg) {
            if ((this.BatchExecuteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.BatchExecuteCompleted(this, new BatchExecuteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SingleExecute", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int SingleExecute(string code, string[] inputVars, string[] inputs, string[] outputVars, out string outputs, out string errors) {
            object[] results = this.Invoke("SingleExecute", new object[] {
                        code,
                        inputVars,
                        inputs,
                        outputVars});
            outputs = ((string)(results[1]));
            errors = ((string)(results[2]));
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void SingleExecuteAsync(string code, string[] inputVars, string[] inputs, string[] outputVars) {
            this.SingleExecuteAsync(code, inputVars, inputs, outputVars, null);
        }
        
        /// <remarks/>
        public void SingleExecuteAsync(string code, string[] inputVars, string[] inputs, string[] outputVars, object userState) {
            if ((this.SingleExecuteOperationCompleted == null)) {
                this.SingleExecuteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSingleExecuteOperationCompleted);
            }
            this.InvokeAsync("SingleExecute", new object[] {
                        code,
                        inputVars,
                        inputs,
                        outputVars}, this.SingleExecuteOperationCompleted, userState);
        }
        
        private void OnSingleExecuteOperationCompleted(object arg) {
            if ((this.SingleExecuteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SingleExecuteCompleted(this, new SingleExecuteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void VersionCompletedEventHandler(object sender, VersionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class VersionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal VersionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void ExecuteCompletedEventHandler(object sender, ExecuteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExecuteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ExecuteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public object retval {
            get {
                this.RaiseExceptionIfNecessary();
                return ((object)(this.results[1]));
            }
        }
        
        /// <remarks/>
        public string error {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[2]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void BatchExecuteCompletedEventHandler(object sender, BatchExecuteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class BatchExecuteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal BatchExecuteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public object[] outputs {
            get {
                this.RaiseExceptionIfNecessary();
                return ((object[])(this.results[1]));
            }
        }
        
        /// <remarks/>
        public string[] errors {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[2]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void SingleExecuteCompletedEventHandler(object sender, SingleExecuteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SingleExecuteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SingleExecuteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public string outputs {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
        
        /// <remarks/>
        public string errors {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[2]));
            }
        }
    }
}

#pragma warning restore 1591