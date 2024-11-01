namespace ReceiptPrint
{
    class Bill
    {
        public string order_id { get; set; }
        public int type { get; set; }
        public string counter_id { get; set; }
        public int itr { get; set; }
        public int new_format { get; set; }
        public string token { get; set; }
    }
}
