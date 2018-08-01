using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace XML_Validator
{
    public partial class MainForm : Form
    {
       
        static int errorsCount = 0;
        static string errorsText = "";

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnParseXsd_Click( object sender, EventArgs e )
        {
            string strResult = "";
            txtResult.Text = "";

            if( txtXsdFileName.Text == "" )
            {
                MessageBox.Show( "Please choose Xsd file to validate against!" );
                txtXsdFileName.Focus();
                return;
            }// if

            if( txtXsdSchema.Text == "" )
            {
                MessageBox.Show( "Please enter XML text to validate!" );
                txtXsdSchema.Focus();
                return;
            }// if

            if( !File.Exists( txtXsdFileName.Text ) )
            {
                MessageBox.Show( "Can't find XSD file! File: " + txtXsdFileName.Text );
                return;
            }// if


            if( !ValidateXmlBySchema( txtXsdSchema.Text, txtXsdFileName.Text, ref strResult ) )
            {
                strResult += errorsText;
                txtResult.Text = "Validation Failed! :" + strResult;
                return;
            }


            txtResult.Text = "Validation successful.";
        }

        private void btnChooseXsd_Click( object sender, EventArgs e )
        {
            txtXsdFileName.Text = "";

            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Xsd files (*.xsd)|*.xsd|All files (*.*)|*.*";

            openFileDlg.Title = "Please select an image Xsd fiel to vaidate against.";
            if( openFileDlg.ShowDialog() == DialogResult.OK )
            {
                txtXsdFileName.Text = openFileDlg.FileName;
            }// if
        }


        public bool ValidateXmlBySchema( string xmlInput, string schemaFileName, ref string errorMessage )
        {
            errorsCount = 0;
            errorMessage = "";
            errorsText = "";
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += new ValidationEventHandler( UpdateRatingsValidationHandler );

                string schemaFullFileName = schemaFileName;

                settings.Schemas.Add( null, schemaFullFileName );

                XmlReader reader = XmlReader.Create( new System.IO.StringReader( xmlInput ),
                                                        settings );
                // Parse the file. 
                while( reader.Read() ) ;

                reader.Close();

            }//try
            catch( Exception error )
            {
                // XML Validation failed
                errorMessage = string.Concat( "XML validation failed.\r\nError Message: ", error.Message );
                return false;
            }// catch

            return errorsCount == 0;
        }

        public static void UpdateRatingsValidationHandler( object sender, ValidationEventArgs args )
        {
            errorsCount++;
            errorsText = string.Concat( errorsText,  args.Message + "\r\n"  );
        }

    }
}
