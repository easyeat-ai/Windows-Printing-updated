using System;
using System.Drawing;
using System.Drawing.Printing;

namespace ReceiptPrint
{
    class CashDrawer
    {
        public dynamic cashData;
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

        public CashDrawer(dynamic cashData)
        {
            this.cashData = cashData;
            printerName = cashData.printerName;
            pageWidth = this.findPageWidth();
            fz = (14 * pageWidth) / 300;
            font = new Font("Arial", fz);
            boldfont = new Font("Arial", fz, FontStyle.Bold);
            largefont = new Font("Arial", (int)(1.2 * fz));
            fontheight = font.GetHeight();
            boldfontheight = boldfont.GetHeight();
            largefontheight = largefont.GetHeight();
            p1 = (int)(0.96 * pageWidth);
            sfCenter =  new StringFormat();
            sfNear = new StringFormat();
            sfFar = new StringFormat();
            sfCenter.Alignment = sfCenter.LineAlignment =  StringAlignment.Center;
            sfNear.Alignment = sfNear.LineAlignment =  StringAlignment.Near;
            sfFar.Alignment = sfFar.LineAlignment =  StringAlignment.Far;
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
            data = prefData.ToUpper() + data.ToUpper();
            int dataHeight = getStringHeight(data, largefont);
            rect = new Rectangle(startX, startY + offset, p1, dataHeight);

            if (flag) 
                graphic.FillRectangle(blackBrush, rect);

            if(flag)
                graphic.DrawString(data, largefont, Brushes.White, rect, sf);
            else 
                graphic.DrawString(data, largefont, Brushes.Black, rect, sf);

            offset += (int)dataHeight + 3;
        }
        public void cashDocumnetPrintPage(PrintPageEventArgs e)
        {
            graphic = e.Graphics;
            startX = (int)(0.02 * pageWidth);
            startY = 5;
            offset = 0;

            if (cashData.counterName != null)
                setData((string)cashData.counterName, true, sfCenter);

            offset += 30;

            if (cashData.date != null)
                setData((string)cashData.date, false, sfNear,"Date: ");

            if (cashData.time != null)
                setData((string)cashData.time, false, sfNear, "Time: ");

            if (cashData.name != null)
                setData((string)cashData.name, false, sfNear, "Staff Name: ");

            if (cashData.amount != null)
                setData((string)cashData.amount, false, sfNear, "Amount: ");

            if (cashData.reason != null)
                setData((string)cashData.reason, false, sfNear,"Reason: ");

            if (cashData.opening_cash_float != null)
                setData((string)cashData.opening_cash_float, false, sfNear, "Cash Float: ");

            if (cashData.total_cash_in != null)
                setData((string)cashData.total_cash_in, false, sfNear, "Total Cash In: ");


            if (cashData.cash_in_sales != null)
                setData((string)cashData.cash_in_sales, false, sfNear, "Cash In Sales: ");

            if (cashData.cash_in_others != null)
                setData((string)cashData.cash_in_others, false, sfNear, "Cash In Others: ");

            if (cashData.total_cash_out != null)
                setData((string)cashData.total_cash_out, false, sfNear, "Total Cash Out: ");

            if (cashData.net_cash_balance != null)
                setData((string)cashData.net_cash_balance, false, sfNear, "Net Cash Balance: ");

            if (cashData.expected_cash_in_drawer != null)
                setData((string)cashData.expected_cash_in_drawer, false, sfNear, "Expected Cash In Drawer: ");
            if (cashData.actual_cash_in_drawer != null)
                setData((string)cashData.actual_cash_in_drawer, false, sfNear, "Actual Cash In Drawer: ");

            if (cashData.excess_short_cash != null)
                setData((string)cashData.excess_short_cash, false, sfNear, "Excess/Short Cash: ");

            if (cashData.close_cashier != null)
                setData((string)cashData.close_cashier, false, sfNear, "Close Cashier: ");

            offset += 30;
            setData("", false, sfCenter, "Signed By: ");
        }

        public void finalPrint()
        {
            this.printing = setPrinter();

            this.printing.PrintPage += (sender, args) => this.cashDocumnetPrintPage(args);

            this.printing.Print();
        }
    }

}
