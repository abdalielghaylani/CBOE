begin
	coedb.biosar_utils.createOrUpdateMasterFromBioSAR;
end;
/

begin

coedb.biosar_utils.createOrUpdateDVFromBioSAR(
									 'DV with 10 tables',
									 'BioDM 10 child',
									 '1',
									 1200,
									 'BIODM',
									 'DGB10',
									 'AKT,DGB50,DGB20,DGB10,DGB100,Ad Hoc Assays,Amgen/ARRY-403 Backup,Aurora,BACE,CETP,CHK1,COT,Calcineurin,DP2,EG5/Kinesin,EGFR,ERK,EphA2,ErbB2,FAK,GNE General,GP-1,GPR119,Glucokinase,HSP90,IGFR,IL2,IP Biology,In Vitro Outsourced,Intermune HCV,JAK3,JNK,LPAR1,MAK2/MKK,MEK,MK2,MMP13,Mer,Outsourced,Outsourced Kinase Screens,PDE,PDGFR/FLT3,PGE Synthase,Pim Kinase,QLT ILK,SHP2,STK33,Soft Taxol,Structure Biology,Syk,TBK1,TGR5,TLR8,TNIK,TRKA,TYK2,Tankyrase,VEGF/KDR,Wuxi,bRAF,cFMS,cMet,mTOR,p38');

end;
/
