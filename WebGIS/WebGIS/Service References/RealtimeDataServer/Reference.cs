﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.1026
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebGIS.RealtimeDataServer {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CarRealData", Namespace="http://schemas.datacontract.org/2004/07/RealtimeDataService")]
    [System.SerializableAttribute()]
    public partial class CarRealData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long AlarmField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long Alarm808Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long AlarmExt808Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AlarmStrField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int AltitudeMetersField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CarNumField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long CaridField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int HeadingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HeadingStrField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LatiField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LongField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int OnlineStatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string OnlineStatusStrField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private float SpeedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long StatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long Status808Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long StatusExt808Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StatusStrField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int SumMilesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime TDateTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long TNOField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] sPositionAdditionalInfoField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Alarm {
            get {
                return this.AlarmField;
            }
            set {
                if ((this.AlarmField.Equals(value) != true)) {
                    this.AlarmField = value;
                    this.RaisePropertyChanged("Alarm");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Alarm808 {
            get {
                return this.Alarm808Field;
            }
            set {
                if ((this.Alarm808Field.Equals(value) != true)) {
                    this.Alarm808Field = value;
                    this.RaisePropertyChanged("Alarm808");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long AlarmExt808 {
            get {
                return this.AlarmExt808Field;
            }
            set {
                if ((this.AlarmExt808Field.Equals(value) != true)) {
                    this.AlarmExt808Field = value;
                    this.RaisePropertyChanged("AlarmExt808");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AlarmStr {
            get {
                return this.AlarmStrField;
            }
            set {
                if ((object.ReferenceEquals(this.AlarmStrField, value) != true)) {
                    this.AlarmStrField = value;
                    this.RaisePropertyChanged("AlarmStr");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int AltitudeMeters {
            get {
                return this.AltitudeMetersField;
            }
            set {
                if ((this.AltitudeMetersField.Equals(value) != true)) {
                    this.AltitudeMetersField = value;
                    this.RaisePropertyChanged("AltitudeMeters");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CarNum {
            get {
                return this.CarNumField;
            }
            set {
                if ((object.ReferenceEquals(this.CarNumField, value) != true)) {
                    this.CarNumField = value;
                    this.RaisePropertyChanged("CarNum");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Carid {
            get {
                return this.CaridField;
            }
            set {
                if ((this.CaridField.Equals(value) != true)) {
                    this.CaridField = value;
                    this.RaisePropertyChanged("Carid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Heading {
            get {
                return this.HeadingField;
            }
            set {
                if ((this.HeadingField.Equals(value) != true)) {
                    this.HeadingField = value;
                    this.RaisePropertyChanged("Heading");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HeadingStr {
            get {
                return this.HeadingStrField;
            }
            set {
                if ((object.ReferenceEquals(this.HeadingStrField, value) != true)) {
                    this.HeadingStrField = value;
                    this.RaisePropertyChanged("HeadingStr");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Lati {
            get {
                return this.LatiField;
            }
            set {
                if ((this.LatiField.Equals(value) != true)) {
                    this.LatiField = value;
                    this.RaisePropertyChanged("Lati");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Long {
            get {
                return this.LongField;
            }
            set {
                if ((this.LongField.Equals(value) != true)) {
                    this.LongField = value;
                    this.RaisePropertyChanged("Long");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int OnlineStatus {
            get {
                return this.OnlineStatusField;
            }
            set {
                if ((this.OnlineStatusField.Equals(value) != true)) {
                    this.OnlineStatusField = value;
                    this.RaisePropertyChanged("OnlineStatus");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OnlineStatusStr {
            get {
                return this.OnlineStatusStrField;
            }
            set {
                if ((object.ReferenceEquals(this.OnlineStatusStrField, value) != true)) {
                    this.OnlineStatusStrField = value;
                    this.RaisePropertyChanged("OnlineStatusStr");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public float Speed {
            get {
                return this.SpeedField;
            }
            set {
                if ((this.SpeedField.Equals(value) != true)) {
                    this.SpeedField = value;
                    this.RaisePropertyChanged("Speed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Status {
            get {
                return this.StatusField;
            }
            set {
                if ((this.StatusField.Equals(value) != true)) {
                    this.StatusField = value;
                    this.RaisePropertyChanged("Status");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Status808 {
            get {
                return this.Status808Field;
            }
            set {
                if ((this.Status808Field.Equals(value) != true)) {
                    this.Status808Field = value;
                    this.RaisePropertyChanged("Status808");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long StatusExt808 {
            get {
                return this.StatusExt808Field;
            }
            set {
                if ((this.StatusExt808Field.Equals(value) != true)) {
                    this.StatusExt808Field = value;
                    this.RaisePropertyChanged("StatusExt808");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StatusStr {
            get {
                return this.StatusStrField;
            }
            set {
                if ((object.ReferenceEquals(this.StatusStrField, value) != true)) {
                    this.StatusStrField = value;
                    this.RaisePropertyChanged("StatusStr");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SumMiles {
            get {
                return this.SumMilesField;
            }
            set {
                if ((this.SumMilesField.Equals(value) != true)) {
                    this.SumMilesField = value;
                    this.RaisePropertyChanged("SumMiles");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime TDateTime {
            get {
                return this.TDateTimeField;
            }
            set {
                if ((this.TDateTimeField.Equals(value) != true)) {
                    this.TDateTimeField = value;
                    this.RaisePropertyChanged("TDateTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long TNO {
            get {
                return this.TNOField;
            }
            set {
                if ((this.TNOField.Equals(value) != true)) {
                    this.TNOField = value;
                    this.RaisePropertyChanged("TNO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] sPositionAdditionalInfo {
            get {
                return this.sPositionAdditionalInfoField;
            }
            set {
                if ((object.ReferenceEquals(this.sPositionAdditionalInfoField, value) != true)) {
                    this.sPositionAdditionalInfoField = value;
                    this.RaisePropertyChanged("sPositionAdditionalInfo");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CarTransmissionProtocolInfo", Namespace="http://schemas.datacontract.org/2004/07/RealtimeDataService")]
    [System.SerializableAttribute()]
    public partial class CarTransmissionProtocolInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<int, byte[]> TransDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long cidField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long tnoField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<int, byte[]> TransData {
            get {
                return this.TransDataField;
            }
            set {
                if ((object.ReferenceEquals(this.TransDataField, value) != true)) {
                    this.TransDataField = value;
                    this.RaisePropertyChanged("TransData");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long cid {
            get {
                return this.cidField;
            }
            set {
                if ((this.cidField.Equals(value) != true)) {
                    this.cidField = value;
                    this.RaisePropertyChanged("cid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long tno {
            get {
                return this.tnoField;
            }
            set {
                if ((this.tnoField.Equals(value) != true)) {
                    this.tnoField = value;
                    this.RaisePropertyChanged("tno");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RealtimeDataServer.IWCFService")]
    public interface IWCFService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFService/DoWork", ReplyAction="http://tempuri.org/IWCFService/DoWorkResponse")]
        int DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFService/GetOnlineCars", ReplyAction="http://tempuri.org/IWCFService/GetOnlineCarsResponse")]
        long[] GetOnlineCars(string syskey);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFService/GetCarData", ReplyAction="http://tempuri.org/IWCFService/GetCarDataResponse")]
        WebGIS.RealtimeDataServer.CarRealData GetCarData(string syskey, long carid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFService/GetCarsData", ReplyAction="http://tempuri.org/IWCFService/GetCarsDataResponse")]
        WebGIS.RealtimeDataServer.CarRealData[] GetCarsData(string syskey, long[] carids);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFService/GetCarTransData", ReplyAction="http://tempuri.org/IWCFService/GetCarTransDataResponse")]
        WebGIS.RealtimeDataServer.CarTransmissionProtocolInfo GetCarTransData(string syskey, long carid);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWCFServiceChannel : WebGIS.RealtimeDataServer.IWCFService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WCFServiceClient : System.ServiceModel.ClientBase<WebGIS.RealtimeDataServer.IWCFService>, WebGIS.RealtimeDataServer.IWCFService {
        
        public WCFServiceClient() {
        }
        
        public WCFServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WCFServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WCFServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WCFServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int DoWork() {
            return base.Channel.DoWork();
        }
        
        public long[] GetOnlineCars(string syskey) {
            return base.Channel.GetOnlineCars(syskey);
        }
        
        public WebGIS.RealtimeDataServer.CarRealData GetCarData(string syskey, long carid) {
            return base.Channel.GetCarData(syskey, carid);
        }
        
        public WebGIS.RealtimeDataServer.CarRealData[] GetCarsData(string syskey, long[] carids) {
            return base.Channel.GetCarsData(syskey, carids);
        }
        
        public WebGIS.RealtimeDataServer.CarTransmissionProtocolInfo GetCarTransData(string syskey, long carid) {
            return base.Channel.GetCarTransData(syskey, carid);
        }
    }
}
