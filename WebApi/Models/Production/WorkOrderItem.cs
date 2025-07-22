using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Production
{


    public class WorkOrderItem
    {
        [Key]
        [Required]
        public int WorkOrderDocEntry { get; set; }
        public int LineNum { get; set; }
        public int VisOrder { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        public decimal BaseQuantity { get; set; }
        public decimal PlannedQuantity { get; set; }

    }// fin de la clase


}// fin del namespace
