//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheHopperService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HistoricalRequest
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public int NumPassangers { get; set; }
        public string Comment { get; set; }
        public int DriverID { get; set; }
        public int RiderID { get; set; }
    }
}
