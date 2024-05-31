using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace pcg.Models
{
    public class SitesModel
    {
        public string SiteId { get; set; }
        [Required(ErrorMessage = "Client is required.")]
        public string Client { get; set; }
        public string Clientcheck { get; set; }
        [Required(ErrorMessage = "Site is required.")]
        public string Site { get; set; }
        public string SiteOM { get; set; }
        public string SiteSOM { get; set; }
        public string SiteSC { get; set; }
        public string SiteTK { get; set; }


        public string Task { get; set; }
        public string TaskId { get; set; }
        public string Remarks { get; set; }
        [Required(ErrorMessage = "Description cannot be empty.")]
        public string Description { get; set; }
        public string Descquery { get; set; }
        public string Descvary { get; set; }
        public string Descdocreq { get; set; }
        public string AssignId { get; set; }
        public string SiteReqId { get; set; }
        public string AddedBy { get; set; }
        public string DateStart { get; set; }
        public string DateFwd { get; set; }
        public string DateRcv { get; set; }
        public string DateClr { get; set; }
    }

    public class SitesModel1
    {
        public string SiteId { get; set; }
       
        public string Client { get; set; }
        public string Clientcheck { get; set; }
      
        public string Site { get; set; }
        public string SiteOM { get; set; }
        public string SiteSOM { get; set; }
        public string SiteSC { get; set; }
        public string SiteTK { get; set; }
    }
}

