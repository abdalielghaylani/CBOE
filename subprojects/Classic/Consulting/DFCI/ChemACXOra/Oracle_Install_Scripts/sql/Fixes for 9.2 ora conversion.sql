-- DGB 6/19/2005
-- In the course of adding foreign key constraints to ChemACX 9.2 Oracle
-- found and/or fixed the following problems
-- Fixes applied to Oracle ChemACX 9.2 database, not to Acces version.

-- Fix 2220 substances with bogus synonyms
update substance set synonymid = null
where csnum in (
select s.csnum from substance s left join acx_synonym y
on s.synonymid = y.synonymid
where y.synonymid is null);

-- Fix bad supplierids in substance
-- find bad supplier ids
select * from substance s left join supplier su
on s.supplierid = su.supplierid
where su.supplierid is null;

-- correct them
update substance set supplierid = 418 where supplierid = 999;
update substance set supplierid = 18 where supplierid = 0;

-- Fix 1 bad supplierid in package
select * from package p left join supplier s
on p.supplierid = s.supplierid
where s.supplierid is null;

update package set supplierid = 230 where packageid = 1377341;

-- Delete 1794 orphaned packages
delete from package where packageid in(
select p.packageid from package p left join product pr
on p.productid = pr.productid
where pr.productid is null);

-- Delete 280 orphaned properties from propertyalpha
delete from propertyalpha where productid in (
select a.productid from propertyalpha a left join product p
on a.productid = p.productid
where p.productid is null);

-- other orphaned properties
delete from PROPERTYALPHA where productid is null;

--- Not fixed but noted:

-- 922 package sizes do not have translation
select distinct p."SIZE" from package p left join  packagesizeconversion c
on upper(p."SIZE") = upper(c.size_fk)
where c.size_fk is null;

-- There a many msdx that could be matched to products and are not
select * from msdx m left join product p
on m.productid = p.productid
where p.productid is null;
