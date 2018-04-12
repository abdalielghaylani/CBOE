create or replace view PACKAGE  AS
SELECT to_number(abc_catalog.ecitemnum) AS packageid,
supplier.name AS suppliername,
supplier.supplierid AS supplierid,
to_number(abc_catalog.ecitemnum) AS productid,
REPLACE(abc_catalog.ecui,' ','') AS "SIZE",
phdout.dsg_form AS container,
null as catalog2num,
to_char(abc_catalog.prprice) AS price,
'dollars' AS currency,
'$' AS csymbol,
null AS price_url,
null AS datecreated,
0 AS IsPriceCd,
0 AS IsPriceWWW,
0 AS IsEComm,
null AS discprice,
null AS savings,
abc_catalog.ecname AS Name,
ecitemnum As catalognum
FROM abc_Catalog, phdout, supplier where abc_catalog.ecndc=phdout.ndc and supplier.supplierid=1;

create or replace view PRODUCT  AS
SELECT to_number(abc_catalog.ecitemnum) AS productid,
supplierid AS supplierid,
abc_catalog.ecname AS prodname,
abc_catalog.ecdesc AS proddescrip,
abc_catalog.ecitemnum as catalognum,
null as concentrationpct,
null as concentrationtag,
null as datecreated,
null as mol_id,
to_number(abc_catalog.ecitemnum) as csnum,
null as productclaimedmw,
null as productlaimedformula,
null as chemclassid,
null as numtoskip,
0 as iswww,
null as picture,
0 as ispicture,
abc_catalog.ecdesc as extended_description,
abc_catalog.ecitemnum as acx_id,
0 as hasmsds
FROM abc_Catalog, phdout, supplier where abc_catalog.ecndc=phdout.ndc and supplier.supplierid=1;

create or replace view acx_synonym as
select 100000000 + to_number(abc_catalog.ecitemnum) as synonymid,
abc_catalog.ecname as name,
'Chemical' as synonymtype,
supplierid as supplierid,
to_number(abc_catalog.ecitemnum) as csnum,
null as datecreated,
supplier.name AS suppliername,
abc_catalog.ecitemnum as acx_id
FROM abc_Catalog, phdout, supplier where abc_catalog.ecndc=phdout.ndc and supplier.supplierid=1
union
select 200000000 + to_number(abc_catalog.ecitemnum) as synonymid,
phdout.brand as name,
'Chemical' as synonymtype,
supplierid as supplierid,
to_number(abc_catalog.ecitemnum) as csnum,
null as datecreated,
supplier.name AS suppliername,
abc_catalog.ecitemnum as acx_id
FROM abc_Catalog, phdout, supplier where abc_catalog.ecndc=phdout.ndc and supplier.supplierid=1  and phdout.brand is not null;

--create or replace view substance as
--select
--to_number(abc_catalog.ecitemnum) as csnum,
--abc_catalog.ecndc as cas,
--null as fema,
--null as mol_id,
--null as synonymid,
--null as datecreated,
--supplier.name as suppliername,
--abc_catalog.ecitemnum as acx_id,
--supplier.supplierid as supplierid,
--1 as hasproducts,
--0 as hasmsds,
--1 as numProducts,
--null as base64_cdx
--FROM abc_Catalog, phdout, supplier where abc_catalog.ecndc=phdout.ndc and --supplier.supplierid=1;



