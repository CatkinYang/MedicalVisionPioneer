//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MedicalVisionPioneer.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HWinfo
    {
        public int HWrecordID { get; set; }
        public int patientId { get; set; }
        public Nullable<int> patientHeight { get; set; }
        public Nullable<int> patientWeight { get; set; }
        public System.DateTime infoCreaterdate { get; set; }
    }
}