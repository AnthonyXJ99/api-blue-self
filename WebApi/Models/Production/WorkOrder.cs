
using BlueSelfCheckout.WebApi.Models.Production;
using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Production
{
    public class WorkOrder
    {

        [Key]
        [Required]
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal PlannedQuantity { get; set; }
        public decimal CompletedQuantity { get; set; }
        public string Status { get; set; }
        public string LinkToObj { get; set; }
        public DateOnly StartDate { get; set; }
        public int StartTime { get; set; }
        public DateOnly EndDate { get; set; }
        public int EndTime { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Comments { get; set; }
        public string Printed { get; set; }
        public string DataSource { get; set; }


        // Enlaza con los detalles

        /// <summary>
        /// WORK1 las lineas y los posibles productos que puedan agregar
        /// </summary>
        public List<WorkOrderItem> Items { get; set; } = new List<WorkOrderItem>();



    }// fin de la clase




}// fin del namespace
