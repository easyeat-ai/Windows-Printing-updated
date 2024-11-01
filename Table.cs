using System;
using System.Drawing;
using System.Drawing.Printing;

namespace ReceiptPrint
{
    class Table
    {
        public dynamic tableData;
        public string printerName;
        public int pageWidth, fz;
        public PrintDocument printing;
        public Font font, boldfont, largefont;
        public float fontheight, boldfontheight, largefontheight;
        public int startX, startY, offset, p1;
        public StringFormat sfCenter, sfNear, sfFar;
        SolidBrush blackBrush;
        public Rectangle rect;
        public SizeF stringSize;
        Graphics graphic;

        public Table(dynamic tableData)
        {
            this.tableData = tableData;
            printerName = tableData.printerName;
            pageWidth = this.findPageWidth();
            fz = (14 * pageWidth) / 300;
            font = new Font("Arial", fz);
            boldfont = new Font("Arial", fz, FontStyle.Bold);
            largefont = new Font("Arial", (int)(1.2 * fz));
            fontheight = font.GetHeight();
            boldfontheight = boldfont.GetHeight();
            largefontheight = largefont.GetHeight();
            p1 = (int)(0.96 * pageWidth);
            sfCenter = new StringFormat();
            sfNear = new StringFormat();
            sfFar = new StringFormat();
            sfCenter.Alignment = sfCenter.LineAlignment = StringAlignment.Center;
            sfNear.Alignment = sfNear.LineAlignment = StringAlignment.Near;
            sfFar.Alignment = sfFar.LineAlignment = StringAlignment.Far;
            blackBrush = new SolidBrush(Color.Black);
        }

        public int findPageWidth()
        {
            int pwidth;
            PrintDocument pr = new PrintDocument();
            string defaultPrinterName = pr.PrinterSettings.PrinterName;

            pr.PrinterSettings.PrinterName = printerName;
            if (pr.PrinterSettings.IsValid && printerName != null)
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
            pr.PrinterSettings.PrinterName = printerName;
            if (pr.PrinterSettings.IsValid && printerName != null)
                return pr;

            pr.PrinterSettings.PrinterName = defaultPrinterName;
            pr.DefaultPageSettings.PaperSize = new PaperSize("Custom", pageWidth, 5 * 2400);
            Console.WriteLine(pageWidth);
            return pr;
        }

        public int getStringHeight(string data, Font fs)
        {
            stringSize = graphic.MeasureString(data, fs, p1);
            return (int)stringSize.Height;
        }

        public void setData(string data, Boolean flag, StringFormat sf, string prefData = "")
        {
            int dataHeight = getStringHeight(data, largefont);
            rect = new Rectangle(startX, startY + offset, p1, dataHeight);

            if (flag)
                graphic.FillRectangle(blackBrush, rect);

            if (flag)
                graphic.DrawString(data, largefont, Brushes.White, rect, sf);
            else
                graphic.DrawString(data, largefont , Brushes.Black, rect, sf);

            offset += (int)dataHeight + 3;
        }
        public void tableDocumnetPrintPage(PrintPageEventArgs e)
        {
            graphic = e.Graphics;
            startX = (int)(0.02 * pageWidth);
            startY = 5;
            offset = 0;

            setData("TABLE TRANSFER", true, sfCenter);
            offset += 30;

            string str = "OLD: " + tableData.body.Old_Table.ToString() + "    ------>    " + "NEW: " + tableData.body.Table.ToString();
            setData(str, false, sfNear) ;
                
        }

        public void finalPrint()
        {
            this.printing = setPrinter();

            this.printing.PrintPage += (sender, args) => this.tableDocumnetPrintPage(args);

            this.printing.Print();
        }

        public void finalDynamicQRPrint()
        {
            this.printing = setPrinter();

            this.printing.PrintPage += (sender, args) => this.tableDocumnetPrintPage(args);

            this.printing.Print();
        }
    }
}

