//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HopperBackend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RiderReview
    {
        public int ID { get; set; }
        public Nullable<int> Rateing { get; set; }
        public string Comment { get; set; }
        public int DriverID { get; set; }
        public int RiderID { get; set; }
    }
}
