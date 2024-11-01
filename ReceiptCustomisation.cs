
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ReceiptPrint
{
    class ReceiptCustomisation
    {
        public dynamic receiptobj;
        const String underline = "------------------------------------------------------------------------------------------------------------------------------------------";

        public ReceiptCustomisation(dynamic currobj)
        {
            this.receiptobj = currobj;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void receiptDocumentPrintPage(PrintPageEventArgs e, int pageWidth)
        {
            dynamic Receiptdata = this.receiptobj;
            int fz = (10 * pageWidth) / 300;
            Font font = new Font("Arial", fz + 2); //Normal Font
            Font Bfont = new Font("Arial", fz, FontStyle.Bold); // Normal Font + Bold
            Font Sfont = new Font("Arial", (int)(1.4 * fz)); // Large Font
            Font italicFont = new Font("Arial", fz, FontStyle.Italic); //Normal Font + Italic
            Font largeFont = new Font("Arial", (int)(1.8 * fz));
            Graphics graphic = e.Graphics;
            int startX = 2;
            int startY = 5;
            int offset = 0;
            int p1 = (int)(0.48 * pageWidth), p2 = (int)(0.17 * pageWidth), p3 = (int)(0.1 * pageWidth), p4 = (int)(0.25 * pageWidth);
            float fontheight = font.GetHeight();
            float Bfontheight = Bfont.GetHeight();
            float Sfontheight = Sfont.GetHeight();
            float largefontheight = largeFont.GetHeight();
            Rectangle rect;
            SizeF stringSize = new SizeF();
            StringFormat sf1 = new StringFormat();
            sf1.Alignment = StringAlignment.Center;
            StringFormat sf2 = new StringFormat();
            sf2.Alignment = StringAlignment.Near;
            StringFormat sf3 = new StringFormat();
            sf3.Alignment = StringAlignment.Far;

            if (Receiptdata.logo != null)
            {
                // Create Image
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(Receiptdata.logo.ToString());
                var img = Image.FromStream(stream);

                //Resize Image
                Bitmap bmap = ResizeImage(img, 100, 100);

                //Draw Image
                rect = new Rectangle(startX + (pageWidth - 100) / 2, startY, 100, 100);
                graphic.DrawImage(bmap, rect);
                offset = offset + 110;

            }

            if (Receiptdata.header != null)
            {
                rect = new Rectangle(startX, startY + offset, pageWidth, 0);
                foreach (string cstr in Receiptdata.header)
                {

                    offset += (int)Bfontheight + 5;
                    rect.Y = startY + offset;
                    graphic.DrawString(cstr, Bfont, new SolidBrush(Color.Black), rect, sf1);
                }
                offset += (int)Bfontheight + 5;
            }
            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 5;

            if (Receiptdata.body != null)
            {
                if (Receiptdata.body.Order_seq != null)
                {
                    string stritem1 = "ORDER: " + Receiptdata.body.Order_seq;
                    stringSize = graphic.MeasureString(stritem1, Bfont, pageWidth - 10);
                    rect = new Rectangle(startX, startY + offset, pageWidth - 10, (int)stringSize.Height);
                    graphic.DrawString(stritem1, Bfont, Brushes.Black, rect, sf1);
                    offset = offset + (int)stringSize.Height + 1;

                    graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
                    offset += (int)Bfontheight + 3;
                }
                string stritem = "Table: " + Receiptdata.body.Table;
                stringSize = graphic.MeasureString(stritem, Sfont, pageWidth - 10);
                rect = new Rectangle(startX, startY + offset, pageWidth - 10, (int)stringSize.Height);
                graphic.DrawString(stritem, Sfont, Brushes.Black, rect, sf2);
                offset = offset + (int)stringSize.Height + 3;

                graphic.DrawString("Invoice No: " + Receiptdata.body.Invoice_No, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)Sfontheight + 3;

                graphic.DrawString("Date: " + Receiptdata.body.Date, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)Sfontheight + 3;

                if (!String.IsNullOrEmpty(Receiptdata.body.Cashier.ToString()))
                {
                    graphic.DrawString("Cashier: " + Receiptdata.body.Cashier, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)Sfontheight + 3;
                }
                if (Receiptdata.body.uname != null)
                {
                    graphic.DrawString("Name: " + Receiptdata.body.uname, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)Sfontheight + 3;
                }
                if (Receiptdata.body.mob != null)
                {
                    graphic.DrawString("Phone: " + Receiptdata.body.mob, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)Sfontheight + 3;
                }
                if (Receiptdata.body.Payment_Type != null)
                {
                    graphic.DrawString("Payment_Type: " + Receiptdata.body.Payment_Type, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)Sfontheight + 3;
                }

                if (Receiptdata.body.PAX != null)
                {
                    graphic.DrawString("PAX: " + Receiptdata.body.PAX, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)Sfontheight + 3;
                }

            }

            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 5;


            if (Receiptdata.order.items != null)
            {
                Rectangle rect1, rect2, rect3, rect4;
                int offsetx = 0, pd = (int)(0.005 * pageWidth);

                //Headers for items
                rect1 = new Rectangle(startX + offsetx, startY + offset, p1 + pd, (int)Bfontheight);
                graphic.DrawString("Item", Bfont, Brushes.Black, rect1, sf2);
                offsetx += p1 + pd + 1;

                rect2 = new Rectangle(startX + offsetx, startY + offset, p2, (int)Bfontheight);
                graphic.DrawString("Pr", Bfont, Brushes.Black, rect2, sf2);
                offsetx += p2 + pd;

                rect3 = new Rectangle(startX + offsetx, startY + offset, p3, (int)Bfontheight);
                graphic.DrawString("Qty", Bfont, Brushes.Black, rect3, sf2);
                offsetx += p3 + pd;

                rect4 = new Rectangle(startX + offsetx, startY + offset, p4, (int)Bfontheight);
                graphic.DrawString("Amt", Bfont, Brushes.Black, rect4, sf2);

                offset += (int)Bfontheight + 3;
                graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
                offset += (int)fontheight + 3;


                foreach (var pstr in Receiptdata.order.items)
                {
                    string stritem = pstr.name.ToString();
                    string addon = pstr.addon.ToString();
                    string variant = pstr.variant.ToString();

                    /*Printing the Name of the item*/
                    offsetx = 0;
                    stringSize = graphic.MeasureString(stritem, Sfont, p1 + pd);

                    rect1 = new Rectangle(startX + offsetx, startY + offset, p1 + pd, (int)stringSize.Height);
                    graphic.DrawString(stritem, Sfont, Brushes.Black, rect1, sf2);
                    offsetx += p1 + pd + 1;

                    rect2 = new Rectangle(startX + offsetx, startY + offset, p2, (int)fontheight);
                    graphic.DrawString(pstr.price.ToString(), font, Brushes.Black, rect2, sf2);
                    offsetx += p2 + pd;

                    rect3 = new Rectangle(startX + offsetx, startY + offset, p3, (int)fontheight);
                    graphic.DrawString(pstr.qty.ToString(), font, Brushes.Black, rect3, sf2);
                    offsetx += p3 + pd;

                    rect4 = new Rectangle(startX + offsetx, startY + offset, p4, (int)fontheight);
                    graphic.DrawString(pstr.amount.ToString(), font, Brushes.Black, rect4, sf2);
                    offset += (int)stringSize.Height + 3;

                    /*Printing variants, addons and notes*/
                    offsetx = 0;
                    stritem = "";
                    if (!String.IsNullOrEmpty(variant))
                    {
                        stritem += "Variants: " + variant;
                    }

                    if (!String.IsNullOrEmpty(addon))
                    {
                        stritem += (String.IsNullOrEmpty(stritem) ? "" : "\n");
                        stritem += "Addons: " + addon;
                    }

                    stringSize = graphic.MeasureString(stritem, italicFont, p1 + pd);
                    rect1 = new Rectangle(startX + offsetx, startY + offset, p1 + pd, (int)stringSize.Height);
                    graphic.DrawString(stritem, italicFont, Brushes.Black, rect1, sf2);
                    offset += (int)stringSize.Height + 3;

                }
            }

            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 3;

            if (Receiptdata.order.bill != null)
            {

                Rectangle rect1, rect2;

                int offsetx, pd = (int)(0.04 * pageWidth);
                string curr = Receiptdata.order.currency;
                curr += ' ';
                foreach (var pstr in Receiptdata.order.bill)
                {
                    offsetx = 0;

                    //Printing Key and Value
                    if ((pstr.name.ToString() == "Total Payable") | (pstr.name.ToString() == "Balance"))
                    {
                        rect1 = new Rectangle(startX + offsetx, startY + offset, p1, (int)largefontheight);
                        graphic.DrawString(pstr.name.ToString(), largeFont, Brushes.Black, rect1, sf2);
                        offsetx += p1 + pd;

                        rect2 = new Rectangle(startX + offsetx, startY + offset, p2 + p3 + p4, (int)largefontheight);
                        graphic.DrawString(curr + pstr.value.ToString(), largeFont, Brushes.Black, rect2, sf2);
                        offset += (int)largefontheight + 10;
                    }

                    else if ((pstr.name.ToString() == "Payment Mode") | (pstr.name.ToString() == "Transaction ID"))
                    {
                        rect1 = new Rectangle(startX + offsetx, startY + offset, p1, (int)Sfontheight);
                        graphic.DrawString(pstr.name.ToString(), Sfont, Brushes.Black, rect1, sf2);
                        offsetx += p1 + pd;

                        rect2 = new Rectangle(startX + offsetx, startY + offset, p2 + p3 + p4, (int)Sfontheight);
                        graphic.DrawString(pstr.value.ToString(), Sfont, Brushes.Black, rect2, sf2);
                        offset += (int)Sfontheight + 10;
                    }

                    else
                    {
                        stringSize = graphic.MeasureString(pstr.name.ToString(), Sfont, p1 + pd + 1);
                        rect1 = new Rectangle(startX + offsetx, startY + offset, p1 + pd, (int)stringSize.Height);
                        graphic.DrawString(pstr.name.ToString(), Sfont, Brushes.Black, rect1, sf2);
                        offsetx += p1 + pd + 3;

                        rect2 = new Rectangle(startX + offsetx, startY + offset, p2 + p3 + p4, (int)Sfontheight);
                        graphic.DrawString(curr + pstr.value.ToString(), Sfont, Brushes.Black, rect2, sf2);
                        offset += (int)stringSize.Height + 8;

                    }
                }
            }

            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 5;

            if (Receiptdata.footer != null)
            {
                foreach (var pstr in Receiptdata.footer)
                {
                    stringSize = graphic.MeasureString(pstr.ToString(), Bfont, pageWidth);
                    rect = new Rectangle(startX, startY + offset, pageWidth, (int)stringSize.Height);
                    graphic.DrawString(pstr.ToString(), Bfont, Brushes.Black, rect, sf2);
                    offset += (int)stringSize.Height + 5;
                }

            }

        }

        public void finalPrint()

        {
            //Finding Default printer
            PrintDocument pr = new PrintDocument();
            string defaultPrinterName = pr.PrinterSettings.PrinterName;
            int pageWidth;

            //Selecting the Required Printer and find the exact width of the page for it
            pr.PrinterSettings.PrinterName = this.receiptobj.printerName;
            if (pr.PrinterSettings.IsValid && this.receiptobj.printerName != null)
            {
                pageWidth = pr.DefaultPageSettings.PaperSize.Width;
            }
            else
            {
                pr.PrinterSettings.PrinterName = defaultPrinterName;
                pageWidth = 234;
                pr.DefaultPageSettings.PaperSize = new PaperSize("Custom", pageWidth, 5 * 2400);

            }

            //Generating final document for the particular type of the counter
            pr.PrintPage += (sender, args) => receiptDocumentPrintPage(args, pageWidth);

            //Printing the actual Receipt
            Console.WriteLine(pageWidth);
            pr.Print();

        }
    }
}
