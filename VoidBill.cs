namespace ReceiptPrint
{
    class VoidBill
    {
        public string order_id { get; set; }
        public int type { get; set; }
        public string oid { get; set; }
        public int qty { get; set; }
        public int new_format { get; set; }
        public string token { get; set; }
    }
}
