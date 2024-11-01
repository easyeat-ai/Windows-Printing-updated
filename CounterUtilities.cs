using System;
using System.Drawing;
using System.Drawing.Printing;
namespace ReceiptPrint
{
    class CounterUtilities
    {
        public dynamic counterobj;
        public String underline = "------------------------------------------------------------------------------------------------------------------------------------------";
        //public Boolean isVoided;
        public int pageWidth, fz;
        public Font font, Bfont, Sfont, cutFont, icutFont, italicFont;
        public int startX, startY, offset, p1, p2;
        public float fontheight, Bfontheight, Sfontheight;
        public Rectangle rect;
        public SizeF stringSize;
        public StringFormat sf1, sf2, sf3;
        public PrintDocument printing;
        public string printerName;

        public CounterUtilities(dynamic currobj)
        {

            this.counterobj = currobj;
            this.printerName = currobj.printerName;
            this.pageWidth = findPageWidth();
            this.fz = (14 * pageWidth) / 300;
            this.font = new Font("Arial", fz);
            this.Bfont = new Font("Arial", fz, FontStyle.Bold);
            this.Sfont = new Font("Arial", (int)(1.5 * fz));
            this.cutFont = new Font("Arial", fz, FontStyle.Strikeout);
            this.icutFont = new Font("Arial", (int)(fz * 0.8), FontStyle.Strikeout | FontStyle.Italic);
            this.italicFont = new Font("Arial", (int)(fz * 0.8), FontStyle.Italic);
            this.p1 = (int)(0.8 * pageWidth);
            this.p2 = (int)(0.15 * pageWidth);
            this.fontheight = font.GetHeight();
            this.Bfontheight = Bfont.GetHeight();
            this.Sfontheight = Sfont.GetHeight();
            this.stringSize = new SizeF();
            this.sf1 = new StringFormat();
            sf1.Alignment = StringAlignment.Center;
            this.sf2 = new StringFormat();
            sf2.Alignment = StringAlignment.Near;
            this.sf3 = new StringFormat();
            sf3.Alignment = StringAlignment.Far;
        }

        public void CounterDocumentPrintPage(PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;
            this.startX = 5;
            this.startY = 5;
            this.offset = 0;
            if (counterobj.counterName.ToString() != null)
            {
                rect = new Rectangle(startX, startY + offset, pageWidth, 0);
                offset = offset + (int)Bfontheight + 3;
                rect.Y = startY + offset;
                graphic.DrawString(counterobj.counterName.ToString(), Bfont, Brushes.Black, rect, sf1);
            }
            offset = offset + (int)Bfontheight + 3;
            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 5;

            if (counterobj.body != null)
            {
                if (counterobj.body.Order_seq != null)
                {
                    string stritem1 = "ORDER: " + counterobj.body.Order_seq;
                    stringSize = graphic.MeasureString(stritem1, Bfont, pageWidth - 10);
                    rect = new Rectangle(startX, startY + offset, pageWidth - 10, (int)stringSize.Height);
                    graphic.DrawString(stritem1, Bfont, Brushes.Black, rect, sf1);
                    offset = offset + (int)stringSize.Height + 1;

                    graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
                    offset += (int)Bfontheight + 3;
                }
                string stritem = "Table: " + counterobj.body.Table;
                stringSize = graphic.MeasureString(stritem, Sfont, pageWidth - 10);
                rect = new Rectangle(startX, startY + offset, pageWidth - 10, (int)stringSize.Height);
                graphic.DrawString(stritem, Sfont, Brushes.Black, rect, sf2);
                offset = offset + (int)stringSize.Height + 3;

                //graphic.DrawString("Table: " + counterobj.body.Table, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                //offset = offset + (int)Sfontheight + 3;

                graphic.DrawString("Invoice No: " + counterobj.body.Invoice_No, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontheight + 3;

                DateTime now = DateTime.Now;
                string currdate = now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Year.ToString();
                string currtime = now.Hour.ToString() + ":" + now.Minute.ToString();
                graphic.DrawString("Date: " + currdate, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontheight + 3;

                graphic.DrawString("Time: " + currtime, font, new SolidBrush(Color.Black), startX, startY + offset); ;
                offset = offset + (int)fontheight + 3;

                //Finding Total No. of Items
                int totalItems = 0;
                foreach (var pstr in counterobj.items)
                    totalItems += int.Parse(pstr.qty.ToString());

                graphic.DrawString("Total Items: " + totalItems, font, new SolidBrush(Color.Black), startX, startY + offset); ;
                offset = offset + (int)fontheight + 3;
            }

            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 5;

            if (counterobj.items != null)
            {
                Rectangle rect1, rect2;
                int offsetx = 0, pd = (int)(0.05 * pageWidth);

                //Headers for items
                rect1 = new Rectangle(startX + offsetx, startY + offset, p1 + pd, (int)Bfontheight);
                graphic.DrawString("Item Name", Bfont, Brushes.Black, rect1, sf2);
                offsetx += p1 + pd;

                rect2 = new Rectangle(startX + offsetx, startY + offset, p2, (int)Bfontheight);
                graphic.DrawString("Qty", Bfont, Brushes.Black, rect2, sf2);
                offsetx += p2;


                offset += (int)Bfontheight + 3;
                graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
                offset += (int)fontheight + 3;

                foreach (var pstr in counterobj.items)
                {

                    /*Making the whole item with addon, variants*/
                    string stritem = pstr.name.ToString();
                    string addon = pstr.addon.ToString();
                    string variant = pstr.variant.ToString();
                    string itemnote = pstr.note.ToString();


                    /*Print the name first and then variants, addon, notes with different font*/
                    /*1.Name of the item first*/
                    offsetx = 0;
                    stringSize = graphic.MeasureString(stritem, font, p1);

                    rect1 = new Rectangle(startX + offsetx, startY + offset, p1, (int)stringSize.Height);
                    if (pstr.strike == 1)
                        graphic.DrawString(stritem, cutFont, Brushes.Black, rect1, sf2);
                    else
                        graphic.DrawString(stritem, font, Brushes.Black, rect1, sf2);
                    offsetx += p1 + pd;

                    rect2 = new Rectangle(startX + offsetx, startY + offset, p2, (int)fontheight);
                    graphic.DrawString(pstr.qty.ToString(), font, Brushes.Black, rect2, sf2);
                    offsetx += p2;
                    offset += (int)stringSize.Height + 3;


                    /*2. Variants, Addons, Notes Second*/
                    stritem = "";
                    if (!String.IsNullOrEmpty(variant))
                        stritem += "Variants: " + variant;

                    if (!String.IsNullOrEmpty(addon)){
                        stritem += (String.IsNullOrEmpty(stritem) ? "" : "\n");
                        stritem += "Addons: " + addon;
                    }

                    if (!String.IsNullOrEmpty(itemnote)){
                        stritem += (String.IsNullOrEmpty(stritem) ? "" : "\n");
                        stritem += "Note: " + itemnote;
                    }

                    offsetx = 0;
                    stringSize = graphic.MeasureString(stritem, italicFont, p1);

                    rect1 = new Rectangle(startX + offsetx, startY + offset, p1, (int)stringSize.Height);
                    if (pstr.strike == 1)
                        graphic.DrawString(stritem, icutFont, Brushes.Black, rect1, sf2);
                    else
                        graphic.DrawString(stritem, italicFont, Brushes.Black, rect1, sf2);
                    offsetx += p1 + pd;
                    offset += (int)stringSize.Height + 3;
                }
            }

            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 3;

            String[] note = counterobj.note.ToObject<String[]>();
            if (note.Length != 0)
            {
                graphic.DrawString("Note:", Bfont, Brushes.Black, new Point(startX, startY + offset));
                offset += (int)fontheight + 3;

                rect = new Rectangle(startX, startY + offset, pageWidth, 25);
                string notes = "";
                foreach (var el in note)
                    notes += "-" + el + "\n";
                stringSize = graphic.MeasureString(notes, font, pageWidth);
                rect = new Rectangle(startX, startY + offset, pageWidth, (int)stringSize.Height);
                graphic.DrawString(notes, font, Brushes.Black, rect, sf2);
                offset += (int)stringSize.Height + 5;
            }

            String[] allergic_items = counterobj.allergic_items.ToObject<String[]>();
            if (allergic_items.Length != 0)
            {
                graphic.DrawString("Allergic To:", Bfont, Brushes.Black, new Point(startX, startY + offset));
                offset += (int)fontheight + 3;

                rect = new Rectangle(startX, startY + offset, pageWidth, 0);
                foreach (var el in allergic_items)
                {
                    offset += (int)fontheight + 3;
                    rect.Y = startY + offset;
                    graphic.DrawString("- " + el.ToString(), font, Brushes.Black, rect, sf2);
                }
                offset += (int)fontheight + 3;

                graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            }

            string message = "Thank You";
            graphic.DrawString(message, Bfont, Brushes.Black, new Point(startX, startY + offset + 50));
            offset += (int)fontheight + 3;

        }

        public void CounterDocumentPrintPage(PrintPageEventArgs e, dynamic item)
        {
            Graphics graphic = e.Graphics;
            this.startX = 5;
            this.startY = 5;
            this.offset = 0;

            if (counterobj.counterName.ToString() != null)
            {
                rect = new Rectangle(startX, startY + offset, pageWidth, 0);
                offset = offset + (int)Bfontheight + 3;
                rect.Y = startY + offset;
                graphic.DrawString(counterobj.counterName.ToString(), Bfont, Brushes.Black, rect, sf1);
            }
            offset = offset + (int)Bfontheight + 3;


            if (counterobj.body != null)
            {
                string stritem = "Table: " + counterobj.body.Table;
                stringSize = graphic.MeasureString(stritem, Sfont, pageWidth - 10);
                rect = new Rectangle(startX, startY + offset, pageWidth - 10, (int)stringSize.Height);
                graphic.DrawString(stritem, Sfont, Brushes.Black, rect, sf2);
                offset = offset + (int)stringSize.Height + 3;

                //graphic.DrawString("Table: " + counterobj.body.Table, Sfont, new SolidBrush(Color.Black), startX, startY + offset);
                //offset = offset + (int)Sfontheight + 3;

                graphic.DrawString("Invoice No: " + counterobj.body.Invoice_No, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontheight + 3;

                DateTime now = DateTime.Now;
                string currdate = now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Year.ToString();
                string currtime = now.Hour.ToString() + ":" + now.Minute.ToString();
                graphic.DrawString("Date: " + currdate, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontheight + 3;

                graphic.DrawString("Time: " + currtime, font, new SolidBrush(Color.Black), startX, startY + offset); ;
                offset = offset + (int)fontheight + 3;

                graphic.DrawString("Total Items: 1", font, new SolidBrush(Color.Black), startX, startY + offset); ;
                offset = offset + (int)fontheight + 3;

            }

            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 5;

            if (counterobj.items != null)
            {
                Rectangle rect1, rect2;
                int offsetx = 0, pd = (int)(0.05 * pageWidth);

                //Headers for items
                rect1 = new Rectangle(startX + offsetx, startY + offset, p1 + pd, (int)Bfontheight);
                graphic.DrawString("Item Name", Bfont, Brushes.Black, rect1, sf2);
                offsetx += p1 + pd;

                rect2 = new Rectangle(startX + offsetx, startY + offset, p2, (int)Bfontheight);
                graphic.DrawString("Qty", Bfont, Brushes.Black, rect2, sf2);
                offsetx += p2;


                offset += (int)Bfontheight + 3;
                graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
                offset += (int)fontheight + 3;

                /*Making the whole item with addon, variants*/
                string stritem = item.name.ToString();
                string addon = item.addon.ToString();
                string variant = item.variant.ToString();
                string itemnote = item.note.ToString();


                /*Print the name first and then variants, addon, notes with different font*/
                /*1.Name of the item first*/
                offsetx = 0;
                stringSize = graphic.MeasureString(stritem, font, p1);

                rect1 = new Rectangle(startX + offsetx, startY + offset, p1, (int)stringSize.Height);
                if (item.strike == 1)
                    graphic.DrawString(stritem, cutFont, Brushes.Black, rect1, sf2);
                else
                    graphic.DrawString(stritem, font, Brushes.Black, rect1, sf2);
                offsetx += p1 + pd;

                rect2 = new Rectangle(startX + offsetx, startY + offset, p2, (int)fontheight);
                graphic.DrawString(item.qty.ToString(), font, Brushes.Black, rect2, sf2);
                offsetx += p2;
                offset += (int)stringSize.Height + 3;


                /*2. Variants, Addons, Notes Second*/
                stritem = "";
                if (!String.IsNullOrEmpty(variant))
                    stritem += "Variants: " + variant;

                if (!String.IsNullOrEmpty(addon))
                {
                    stritem += (String.IsNullOrEmpty(stritem) ? "" : "\n");
                    stritem += "Addons: " + addon;
                }

                if (!String.IsNullOrEmpty(itemnote))
                {
                    stritem += (String.IsNullOrEmpty(stritem) ? "" : "\n");
                    stritem += "Note: " + itemnote;
                }

                offsetx = 0;
                stringSize = graphic.MeasureString(stritem, italicFont, p1);

                rect1 = new Rectangle(startX + offsetx, startY + offset, p1, (int)stringSize.Height);
                if (item.strike == 1)
                    graphic.DrawString(stritem, icutFont, Brushes.Black, rect1, sf2);
                else
                    graphic.DrawString(stritem, italicFont, Brushes.Black, rect1, sf2);
                offsetx += p1 + pd;
                offset += (int)stringSize.Height + 3;

            }

            graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            offset += (int)fontheight + 3;

            String[] note = counterobj.note.ToObject<String[]>();
            if (note.Length != 0)
            {
                graphic.DrawString("Note:", Bfont, Brushes.Black, new Point(startX, startY + offset));
                offset += (int)fontheight + 3;

                rect = new Rectangle(startX, startY + offset, pageWidth, 25);
                string notes = "";
                foreach (var el in note)
                    notes += "- " + el + "\n";

                stringSize = graphic.MeasureString(notes, font, pageWidth);
                rect = new Rectangle(startX, startY + offset, pageWidth, (int)stringSize.Height);
                graphic.DrawString(notes, font, Brushes.Black, rect, sf2);
                offset += (int)stringSize.Height + 5;
            }

            String[] allergic_items = counterobj.allergic_items.ToObject<String[]>();
            if (allergic_items.Length != 0)
            {
                graphic.DrawString("Allergic To:", Bfont, Brushes.Black, new Point(startX, startY + offset));
                offset += (int)fontheight + 3;

                rect = new Rectangle(startX, startY + offset, pageWidth, 0);
                foreach (var el in allergic_items)
                {
                    offset += (int)fontheight + 3;
                    rect.Y = startY + offset;
                    graphic.DrawString("- " + el.ToString(), font, Brushes.Black, rect, sf2);
                }
                offset += (int)fontheight + 3;
                graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
            }

            string message = "Thank You";
            graphic.DrawString(message, Bfont, Brushes.Black, new Point(startX, startY + offset + 50));

        }

        public int findPageWidth()
        {
            int pwidth;
            PrintDocument pr = new PrintDocument();
            string defaultPrinterName = pr.PrinterSettings.PrinterName;

            pr.PrinterSettings.PrinterName = this.counterobj.printerName;
            if (pr.PrinterSettings.IsValid && this.counterobj.printerName != null)
                pwidth = pr.DefaultPageSettings.PaperSize.Width;
            else
            {
                pr.PrinterSettings.PrinterName = defaultPrinterName;
                pwidth = 234;
            }
            return pwidth;
        }

        public PrintDocument setPrinter()
        {
            //Finding Default printer
            PrintDocument pr = new PrintDocument();
            string defaultPrinterName = pr.PrinterSettings.PrinterName;

            //Selecting the Required Printer and find the exact width of the page for it
            pr.PrinterSettings.PrinterName = this.printerName;
            if (pr.PrinterSettings.IsValid && this.printerName != null)
                return pr;

            pr.PrinterSettings.PrinterName = defaultPrinterName;
            pr.DefaultPageSettings.PaperSize = new PaperSize("Custom", this.pageWidth, 5 * 2400);
            Console.WriteLine(pageWidth);
            return pr;
        }
    }
}
