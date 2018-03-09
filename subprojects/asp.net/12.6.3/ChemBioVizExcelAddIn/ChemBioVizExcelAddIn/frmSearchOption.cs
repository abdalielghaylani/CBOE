using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;
namespace ChemBioVizExcelAddIn
{
    public partial class frmSearchOption : Form
    {
        public frmSearchOption()
        {
            InitializeComponent();
        }
       

        private void frmSearchOption_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDefaultStructureSearchOption();
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

       //Default setting of search options while loading the form
        private void LoadDefaultStructureSearchOption()
        {
            try
            {
                cbxChargeHeteroAtom.Checked = StructureSearchOption.HitAnyChargeHetero;
                cbxReactionCenter.Checked = StructureSearchOption.ReactionCenter;
                cbxChargeCarbon.Checked = StructureSearchOption.HitAnyChargeCarbon;
                cbxPermitExtraneousFragments.Checked = StructureSearchOption.PermitExtraneousFragments;
                cbxPermitExtraneousFragmentsIfRXN.Checked = StructureSearchOption.PermitExtraneousFragmentsIfRXN;
                cbxFragmentsOverlap.Checked = StructureSearchOption.FragmentsOverlap;
                cbxTautomeric.Checked = StructureSearchOption.Tautomeric;

                //Need discussion
                //cbxFullSearchSimilar.Checked = StructureSearchOption.Similar;

                txtSimilarSearchThld.Text = StructureSearchOption.SimilarSearchThld;

                cbxMatchStereoChem.Checked = StructureSearchOption.MatchTetrahedralStereo;

                if (!cbxMatchStereoChem.Checked)
                {
                    gpbTHDStereo.Enabled = false;
                    gpbDoubleBond.Enabled = false;
                    cbxTHSThickBond.Enabled = false;
                }

                cbxTHSThickBond.Checked = StructureSearchOption.RelativeTetStereo;
                rbtnTHSAny.Checked = StructureSearchOption.TetrahedralStereoHitsAny;
                rbtnTHSEither.Checked = StructureSearchOption.TetrahedralStereoHitsEither;
                rbtnTHSSame.Checked = StructureSearchOption.TetrahedralStereoHitsSame;

                //Need discussion
                rbtnDBAny.Checked = StructureSearchOption.DoubleBondHitsAny;
                rbtnDBSame.Checked = StructureSearchOption.DoubleBondHitsSame;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Set the structure search options setting after closing the form
        private void SetStructureSearchOption()
        {
            try
            {
                //Initiate the arrayList of Structure search options 
                Global.StructureSearchOptions = new ArrayList();

                StructureSearchOption.HitAnyChargeHetero = cbxChargeHeteroAtom.Checked;
                StructureSearchOption.ReactionCenter = cbxReactionCenter.Checked;
                StructureSearchOption.HitAnyChargeCarbon = cbxChargeCarbon.Checked;
                StructureSearchOption.PermitExtraneousFragments = cbxPermitExtraneousFragments.Checked;
                StructureSearchOption.PermitExtraneousFragmentsIfRXN = cbxPermitExtraneousFragmentsIfRXN.Checked;
                StructureSearchOption.FragmentsOverlap = cbxFragmentsOverlap.Checked;

                StructureSearchOption.Tautomeric = cbxTautomeric.Checked;

                StructureSearchOption.SimilarSearchThld = txtSimilarSearchThld.Text;
                StructureSearchOption.MatchTetrahedralStereo = cbxMatchStereoChem.Checked;

                if (StructureSearchOption.MatchTetrahedralStereo)
                {
                    StructureSearchOption.RelativeTetStereo = cbxTHSThickBond.Checked;
                    StructureSearchOption.TetrahedralStereoHitsAny = rbtnTHSAny.Checked;
                    StructureSearchOption.TetrahedralStereoHitsEither = rbtnTHSEither.Checked;
                    StructureSearchOption.TetrahedralStereoHitsSame = rbtnTHSSame.Checked;

                    //StructureSearchOption.DoubleBondHitsAny=rbtnDBAny.Checked;
                    StructureSearchOption.DoubleBondStereo = rbtnDBAny.Checked;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmSearchOption_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SetStructureSearchOption();
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }
        private void txtSimilarSearchThld_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)46 && e.KeyChar != (char)8)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
                e.Handled = true;

        }

        private void cbxMatchStereoChem_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbxMatchStereoChem.Checked)
            {
                gpbTHDStereo.Enabled = false;
                gpbDoubleBond.Enabled = false;
                cbxTHSThickBond.Enabled = false;
            }
            else
            {
                gpbTHDStereo.Enabled = true;
                gpbDoubleBond.Enabled = true;
                cbxTHSThickBond.Enabled = true;
            }
        }
    }
}