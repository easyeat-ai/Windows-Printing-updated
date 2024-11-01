namespace ReceiptPrint
{
    class Dynamic
    {
        public string restaurant_id { get; set; }
        public long expiry_time { get; set; }
        public string table_no { get; set; }
        public string table_id { get; set; }
        public string action { get; set; }
        public int new_format { get; set; }
        public int type { get; set; }
        public string device_id { get; set; }
        public int qrsize { get; set; }
        public string token { get; set; }
    }
}