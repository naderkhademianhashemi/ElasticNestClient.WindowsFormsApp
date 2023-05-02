using ElasticNestClient.WindowsFormsApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElasticNestClient.WindowsFormsApp
{

    public partial class Form1 : Form
    {
        private readonly ElasticSearchClient _esClient;
        public Form1()
        {
            _esClient = new ElasticSearchClient();
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            var p = new ProductDetails()
            {
                ProductId = 1,
                ProductName = "Shirt"
            };

            string term = "Shirt";

            var responsedata = _esClient.EsClient().Search<ProductDetails>(s => s.Source()
                                .Query(q => q
                                .QueryString(qs => qs
                                .AnalyzeWildcard()
                                   .Query(term)
                                   .Fields(fs => fs
                                       .Fields(f1 => f1.ProductName                                               
                                               )
                                   )
                                   )));

            var productDetails = responsedata.Documents.ToList();
        }

            
    }

}


