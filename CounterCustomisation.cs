using System;
using System.Threading.Tasks;

namespace ReceiptPrint
{
    /* This class handles general printing and voided items printing*/
    class CounterCustomisation : CounterUtilities
    {
        public CounterCustomisation(dynamic currobj ) : base((object)currobj)
        { }

        public void finalPrint()
        {
            //Set Printer
            this.printing = setPrinter();

            //Generating final document
            this.printing.PrintPage += (sender, args) => this.CounterDocumentPrintPage(args);

            //Printing the actual Receipt
            this.printing.Print();
        }

    }
}
