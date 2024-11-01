using System;
using System.Drawing;
using System.Drawing.Printing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;


namespace ReceiptPrint
{
    class DynamicQR
    {
        public dynamic receiptobj;
        public Font dfont;

        public DynamicQR(dynamic currobj)
        {
            this.receiptobj = currobj;
        }

        public void dynamicQRPrintPage(PrintPageEventArgs e, int pr_width)
        {
            dynamic Receiptdata = this.receiptobj;
            int offset = 0;
            Graphics graphic = e.Graphics;
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };

            foreach (var item in Receiptdata.data)
            {
                // Heading
                if (item.key == "heading" && item.value is JArray)
                {
                    foreach (var data in item.value)
                    {
                        graphic.DrawString(data.name.ToString(), new Font("Arial", 18, FontStyle.Bold), Brushes.Black, new Rectangle(0, offset, pr_width, 30), centerFormat);
                        offset += 50; // Increase spacing after heading
                    }
                }

                // QR String
                else if (item.key == "qrString" && item.value is JArray)
                {
                    foreach (var data in item.value)
                    {
                        Bitmap qrCodeImage = GenerateQRCode(data.name.ToString());
                        // Calculate the position to center the QR code
                        int qrX = (pr_width - qrCodeImage.Width) / 2;
                        graphic.DrawImage(qrCodeImage, new Point(qrX, offset));
                        offset += qrCodeImage.Height + 10; // Adjust offset after QR code
                    }
                }
                // Table Info
                else if (item.key == "tableInfo" && item.value is JArray)
                {
                    foreach (var data in item.value)
                    {
                        graphic.DrawString(data.name.ToString(), new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Rectangle(0, offset, pr_width, 20), centerFormat);
                        offset += 30; // Spacing for table info
                    }
                }

                // Valid Upto
                else if (item.key == "validUpto" && item.value is JArray)
                {
                    foreach (var data in item.value)
                    {
                        // Change font size and style to make it smaller and lighter
                        graphic.DrawString(data.name.ToString(), new Font("Arial", 10, FontStyle.Regular), Brushes.Gray, new Rectangle(0, offset, pr_width, 20), centerFormat);
                        offset += 10; // Spacing for valid upto
                    }
                }

            }
        }

        public void finalDynamicQRPrint()
        {
            //Finding Default printer
            PrintDocument pr = new PrintDocument();
            string defaultPrinterName = pr.PrinterSettings.PrinterName;
            int pr_width;

            //Selecting the Required Printer and find the exact width of the page for it
            pr.PrinterSettings.PrinterName = this.receiptobj.printerName;
            if (pr.PrinterSettings.IsValid && this.receiptobj.printerName != null)
            {
                pr_width = pr.DefaultPageSettings.PaperSize.Width;
            }
            else
            {
                pr.PrinterSettings.PrinterName = defaultPrinterName;
                pr_width = 280;
                pr.DefaultPageSettings.PaperSize = new PaperSize("Custom", pr_width, 7 * 2400);

            }

            //Generating final document for the particular type of the counter
            pr.PrintPage += (sender, args) => dynamicQRPrintPage(args, pr_width);

            //Printing the actual Receipt
            Console.WriteLine(pr_width);
            pr.Print();
        }


        private Bitmap GenerateQRCode(string data)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                return qrCode.GetGraphic(3); // Adjust size as needed
            }
        }
    }
}