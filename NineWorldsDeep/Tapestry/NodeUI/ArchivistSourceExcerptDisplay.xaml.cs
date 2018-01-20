using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Archivist;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for ArchivistSourceExcerptDisplay.xaml
    /// </summary>
    public partial class ArchivistSourceExcerptDisplay : UserControl
    {
        #region fields

        //private ArchivistSourceExcerptNode sourceExcerptNode;
        private ArchivistSourceExcerpt sourceExcerpt;
        private Db.Sqlite.ArchivistSubsetDb db;

        #endregion

        #region creation

        public ArchivistSourceExcerptDisplay()
        {
            InitializeComponent();
            db = new Db.Sqlite.ArchivistSubsetDb();
        }

        #endregion

        #region public interface

        internal void Display(ArchivistSourceExcerptNode aseNode)
        {
            if (aseNode != null)
            {
                this.sourceExcerpt = aseNode.SourceExcerpt;
            }
            RefreshFromDb();
        }

        /// <summary>
        /// refresh from database by source excerpt id
        /// </summary>
        /// <returns>true if successful, false if unable</returns>
        public bool RefreshFromDb()
        {
            if(this.sourceExcerpt != null && 
                this.sourceExcerpt.SourceExcerptId > 0)
            {
                //some component load partial, just id, need to pull fresh from db
                //this.sourceExcerpt = GenerateDemo();
                this.sourceExcerpt = 
                    db.GetSourceExcerptByIdWithSourceAndAnnotations(this.sourceExcerpt.SourceExcerptId);
                RefreshFromObject();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region demo

        private int demoCount = 0;

        private ArchivistSourceExcerpt GenerateDemo()
        {
            demoCount++;

            var valText = "hard coded demo excerpt value - DEMO COUNT " + demoCount + Environment.NewLine + Environment.NewLine +
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer quis blandit risus. In eget augue id ante convallis volutpat eget vitae ex. Aliquam vehicula justo quis hendrerit eleifend. Ut porttitor placerat leo fringilla faucibus. Phasellus pharetra nunc id tempus vestibulum. Donec sit amet ex ipsum. Sed nec erat dui. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Interdum et malesuada fames ac ante ipsum primis in faucibus. Fusce non lectus eget felis venenatis gravida quis ac turpis. Donec et mi vitae enim tincidunt viverra eu ut velit. Nullam eget rutrum sapien. Ut auctor erat at sem ullamcorper, in condimentum mi congue. Proin non tempus turpis. Etiam sagittis neque eget elit efficitur tempus a et sapien." + Environment.NewLine +
                Environment.NewLine +  "Nam ut sem vulputate, hendrerit est id, scelerisque dui.Cras ultricies, metus eu dictum ultrices, metus magna auctor risus, ut accumsan augue massa eu justo.Pellentesque ac diam quis leo suscipit hendrerit.Ut id neque tempor, condimentum dui ac, euismod nisl.Etiam vitae varius justo, egestas pretium dui. Pellentesque velit ante, faucibus vel purus id, malesuada finibus nunc. Vivamus sit amet venenatis sem.Nulla at justo et massa accumsan ultrices.Vestibulum pharetra tristique tempus. Vestibulum et nisl eu tellus consequat aliquam.Morbi facilisis vehicula ligula, vel suscipit dolor imperdiet a. Mauris congue libero vel eros viverra facilisis.Proin facilisis augue eget congue luctus.";

            var bt = "0:00:00";
            var et = "0:11:11";
            var ep = "111-156";

            if(demoCount > 1)
            {
                bt = "";
                et = "";
            }

            if(demoCount > 2)
            {
                ep = "";
            }

            if(demoCount > 3)
            {
                demoCount = 1;
            }

            var ase = new ArchivistSourceExcerpt()
            {
                SourceExcerptId = 1,
                Source = new ArchivistSource() { Title = "hard coded demo source" },
                ExcerptBeginTime = bt,
                ExcerptEndTime = et,
                ExcerptPages = ep,
                ExcerptValue = valText,
            };

            ase.Annotations.Add(new ArchivistSourceExcerptAnnotation()
            {
                SourceAnnotationValue = "demo annotation hard coded"
            });

            ase.Annotations.Add(new ArchivistSourceExcerptAnnotation()
            {
                SourceAnnotationValue = "demo annotation hard coded two"
            });

            return ase;
        }


        #endregion

        #region private helper methods

        private void RefreshFromObject()
        {
            ccSourceExcerptDetails.Content = null;
            ccSourceExcerptDetails.Content = sourceExcerpt;

            lvSourceExcerptAnnotations.ItemsSource = null;
            if(sourceExcerpt != null)
            {
                lvSourceExcerptAnnotations.ItemsSource = sourceExcerpt.Annotations;
            }
        }

        private void ProcessEntryInput()
        {
            string annotationValue = txtSourceExcerptAnnotationInput.Text;

            if (!string.IsNullOrWhiteSpace(annotationValue))
            {
                if (this.sourceExcerpt != null && 
                    this.sourceExcerpt.SourceExcerptId > 0)
                {
                    //UI.Display.Message("do stuff here. currently using a dummy id and will mess up database if we don't implement the load functionality first");

                    db.InsertSourceExcerptAnnotation(
                        this.sourceExcerpt.SourceExcerptId,
                        annotationValue);

                    RefreshFromDb();

                    txtSourceExcerptAnnotationInput.Text = "";
                }
            }
        }

        #endregion

        #region event handlers

        private void ButtonRefreshSourceExcerpt_Click(object sender, RoutedEventArgs e)
        {
            if (RefreshFromDb())
            {
                UI.Display.Message("refreshed");
            }
            else
            {
                UI.Display.Message("error refreshing from database, possible causes include a null excerpt or excerpt id not being set");
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void btnAddSourceExcerptAnnotation_Click(object sender, RoutedEventArgs e)
        {
            ProcessEntryInput();
        }

        private void txtSourceExcerptAnnotationInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Enter))
            {
                ProcessEntryInput();
                e.Handled = true;
            }
        }

        #endregion
    }
}
