namespace BlueSelfCheckout.WebApi.Models.Sales
{
    public class Invoice
    {
        public int DocEntry { get; set; }
        public string DocNum { get; set; }
        public string Canceled { get; set; }

        public string Printed { get; set; }

        public string DocStatus { get; set; }

        public string ObjType { get; set; }

        public DateOnly DocDate { get; set; }

        public int DocTime { get; set; }
        public DateOnly DocDueDate { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string Address { get; set; }


        public string Comments { get; set; }

        public string SlpCode { get; set; }



    }// fin de la clase


    public class InvoiceItem
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }

        public string BaseDocNum { get; set; }

        public string BaseObjectType { get; set; }

        public int BaseDocEntry  { get; set; }

        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        public decimal Quantity { get; set; }
        public decimal Price { get; set; }// despues del descuento
        public decimal LineTotal { get; set; }
        
        public DateOnly ShipDate { get; set; }

        public string Address { get; set; }

        public string TaxCode { get; set; }

        public string TaxType { get; set; }
        

    }// fin de la clase

}// fin del namespace
