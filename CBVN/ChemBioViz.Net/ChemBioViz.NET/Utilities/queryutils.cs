using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using Infragistics.Win;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;

using CambridgeSoft.COE.Framework.Common;

using FormDBLib;
using CBVUtilities;
using Utilities;

namespace ChemBioViz.NET
{
    #region QueryUtil
    public class QueryUtil
    {
        #region Methods
        //---------------------------------------------------------------------
        private static String ChopAtBracket(String s)
        {
            String sChopped = s;
            int len = s.IndexOf('[');
            if (len > 0)
                sChopped = s.Substring(0, len - 1);
            sChopped.Trim();
            return sChopped;
        }
        //---------------------------------------------------------------------
        public static UltraTreeNode GetSelectedNodeFromTree(UltraExplorerBarGroup gDViews, Point mouseDownPoint)
        {
            UltraTreeNode nodeAtPoint = null;
            UltraExplorerBarContainerControl ubcc = gDViews.Container;
            if (ubcc.Controls.Count > 0)
            {
                UltraTree treeView = ubcc.Controls[0] as UltraTree;
                //Coverity Bug Fix CID :13025 
                if (treeView != null)
                {
                    if (!mouseDownPoint.IsEmpty)
                        nodeAtPoint = treeView.GetNodeFromPoint(mouseDownPoint);
                    else if (treeView.SelectedNodes.Count > 0)
                        nodeAtPoint = treeView.SelectedNodes[0];    // CSBR-118065
                }
            }
            return nodeAtPoint;
        }
        //---------------------------------------------------------------------
        private static SearchCriteria.COEBoolean CFromB(bool b)
        {
            return b ? SearchCriteria.COEBoolean.Yes : SearchCriteria.COEBoolean.No;
        }
        private static bool BFromC(SearchCriteria.COEBoolean c)
        {
            return c == SearchCriteria.COEBoolean.Yes;
        }
        //---------------------------------------------------------------------
        public static void CopySearchOptions(SearchCriteria.StructureCriteria sc, SearchOptionsSettings so, bool bToOpts)
        {
            // this is lousy .. we don't need two different types of data structures for these options, but oh well
            // at the moment only bToOpts=true is used
            if (bToOpts)
            {
                so.AbsoluteHitsRel = BFromC(sc.AbsoluteHitsRel);
                so.FragmentsOverlap = BFromC(sc.FragmentsOverlap);
                so.HitAnyChargeCarbon = BFromC(sc.HitAnyChargeCarbon);
                so.HitAnyChargeHetero = BFromC(sc.HitAnyChargeHetero);
                so.IgnoreImplicitH = BFromC(sc.IgnoreImplicitHydrogens);
                so.PermitExtraneousFragments = BFromC(sc.PermitExtraneousFragments);
                so.ReactionCenter = BFromC(sc.ReactionCenter);
                so.Tautomer = BFromC(sc.Tautometer);

                so.Exact = BFromC(sc.Identity);
                so.FullSearch = BFromC(sc.FullSearch);
                so.Similar = BFromC(sc.Similar);
                so.Substructure = !so.Exact && !so.FullSearch && !so.Similar;
                so.SimThreshold = sc.SimThreshold;

                so.SameDoubleBondStereo = BFromC(sc.DoubleBondStereo);
                so.AnyDoubleBondStereo = !so.SameDoubleBondStereo;

                so.AnyTetStereo = sc.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.Any;
                so.EitherTetStereo = sc.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.Either;
                so.SameTetStereo = sc.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.Same;
                so.MatchStereochemistry = sc.TetrahedralStereo != SearchCriteria.TetrahedralStereoMatching.No;
            }
            else
            {
                sc.AbsoluteHitsRel = CFromB(so.AbsoluteHitsRel);
                sc.FragmentsOverlap = CFromB(so.FragmentsOverlap);
                sc.HitAnyChargeCarbon = CFromB(so.HitAnyChargeCarbon);
                sc.HitAnyChargeHetero = CFromB(so.HitAnyChargeHetero);
                sc.IgnoreImplicitHydrogens = CFromB(so.IgnoreImplicitH);
                sc.PermitExtraneousFragments = CFromB(so.PermitExtraneousFragments);
                sc.ReactionCenter = CFromB(so.ReactionCenter);
                sc.Tautometer = CFromB(so.Tautomer);

                sc.Identity = CFromB(so.Exact);
                sc.FullSearch = CFromB(so.FullSearch);
                sc.Similar = CFromB(so.Similar);
                sc.SimThreshold = so.SimThreshold;

                sc.DoubleBondStereo = CFromB(so.SameDoubleBondStereo);
                sc.TetrahedralStereo = so.AnyTetStereo ? SearchCriteria.TetrahedralStereoMatching.Any :
                                        so.EitherTetStereo ? SearchCriteria.TetrahedralStereoMatching.Either :
                                        so.SameTetStereo ? SearchCriteria.TetrahedralStereoMatching.Same :
                                        SearchCriteria.TetrahedralStereoMatching.No;
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion
}
