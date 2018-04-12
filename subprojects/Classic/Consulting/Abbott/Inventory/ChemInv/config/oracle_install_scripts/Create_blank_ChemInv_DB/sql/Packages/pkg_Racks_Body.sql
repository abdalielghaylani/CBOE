CREATE OR REPLACE PACKAGE BODY RACKS IS

  PROCEDURE DISPLAYRACKGRID(
    p_LocationID inv_locations.location_id%TYPE,
    O_RS OUT CURSOR_TYPE
    ) AS
    BEGIN

		OPEN O_RS FOR

      select (
             select 'Container::'||container_id||'::'||barcode||'::/cheminv/images/listview/flask_closed_icon_16.gif::'||container_name
                    ||'::'|| DECODE(inv_Containers.Qty_Max, NULL, ' ', inv_Containers.Qty_Max||' '||UOM.Unit_Abreviation)
                    ||'::'|| DECODE(inv_Containers.Qty_Remaining, NULL, ' ', inv_Containers.Qty_Remaining||' '||UOM.Unit_Abreviation)
                    ||'::'|| DECODE(inv_Containers.Qty_Available, NULL, ' ', inv_Containers.Qty_Available||' '||UOM.Unit_Abreviation)
                    ||'::'|| DECODE(inv_Containers.Concentration, NULL, ' ', inv_Containers.Concentration||' '||UOC.Unit_Abreviation)
                    ||'::'|| icb.batch_field_1 || '::' || constants.cContainerBatchField1Display
                    ||'::'|| icb.batch_field_2 || '::' || constants.cContainerBatchField2Display
                    ||'::'|| icb.batch_field_3 || '::' || constants.cContainerBatchField3Display
                    ||'::'|| ivrb.RegName
                    ||'::'|| inv_Containers.Qty_Max
                    ||'::'|| inv_Containers.Qty_Remaining
                    ||'::'|| inv_Containers.Qty_Available
                    ||'::'|| inv_Containers.Concentration
                    ||'::'|| ivrb.RegNumber
                    from inv_containers, inv_Units UOM, inv_Units UOC, inv_container_batches icb, inv_vw_reg_batches ivrb
                    where location_id_fk=l.location_id
                    and inv_Containers.UNIT_OF_MEAS_ID_FK = UOM.Unit_ID
                    and inv_Containers.Unit_of_Conc_ID_FK = UOC.Unit_ID(+)
                    and inv_containers.batch_id_fk = icb.batch_id(+)
                    and icb.batch_field_1 = ivrb.RegID(+)
                    and icb.batch_field_2 = ivrb.BatchNumber(+)
                    and rownum < 2
             union
             select 'Plate::'||plate_id||'::'||plate_barcode||'::/cheminv/images/listview/Plate_icon_16.gif::'||plate_name||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::' from inv_plates where location_id_fk=l.location_id and rownum < 2
             union
             select 'Rack::'||location_id||'::'||location_barcode||'::/cheminv/images/treeview/rack_open.gif::'||location_name||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::'||'::' from inv_locations where parent_id=l.location_id and collapse_child_nodes = 1 and rownum < 2             ) as grid_data
            , l.name
            , l.location_id
            , l.location_barcode
            , l.row_index
            , l.col_index
            , l.row_name as rowname
            , l.col_name as colname
            , l.name
            , l.sort_order
            , GUIUTILS.GETRACKLOCATIONPATH(p.location_ID) as location_name
            , GUIUTILS.GETFULLRACKLOCATIONPATH(p.location_ID) as location_name_full
            , f.cell_naming
      from inv_vw_grid_location l, inv_locations p, inv_grid_storage s, inv_grid_format f
      where l.parent_id = p.location_id
      and l.parent_id = s.location_id_fk
      and s.grid_format_id_fk = f.grid_format_id
      and l.parent_id = (p_LocationID);

  END DISPLAYRACKGRID;

  PROCEDURE DISPLAYRACKGRIDCONTAINERS(
    p_LocationID inv_locations.location_id%TYPE,
    O_RS OUT CURSOR_TYPE
    ) AS
    BEGIN

		OPEN O_RS FOR

        select 
          c.barcode as Barcode
          , c.qty_max || (select uom.unit_abreviation from inv_units uom where uom.unit_id = c.unit_of_meas_id_fk) as ContainerSize
          , c.concentration || (select uom.unit_abreviation from inv_units uom where uom.unit_id = c.unit_of_conc_id_fk) as Concentration
          , case 
            when ((select uom.unit_abreviation from inv_units uom where uom.unit_id = c.unit_of_conc_id_fk) = 'mg/ml') and ((select uom.unit_abreviation from inv_units uom where uom.unit_id = c.unit_of_meas_id_fk) = 'ml') then
                c.qty_max*c.concentration || 'mg'
            else
                c.qty_max || (select uom.unit_abreviation from inv_units uom where uom.unit_id = c.unit_of_meas_id_fk)
            end
            as Amount       
          , c.container_name as ContainerName
          , igl.name as GridName
          , ivrb.RegNumber as Batch_Field_1
          , ivrb.BatchNumber as Batch_Field_2
          , ivrb.*
        from inv_containers c, inv_vw_grid_location igl, inv_vw_reg_batches ivrb, inv_container_batches icb
        where igl.parent_id = p_LocationID
        and igl.location_id = c.location_id_fk(+)
        and c.batch_id_fk = icb.batch_id(+)
        and icb.batch_field_1 = ivrb.RegID(+)
        and icb.batch_field_2 = ivrb.BatchNumber(+)
        order by igl.sort_order;

  END DISPLAYRACKGRIDCONTAINERS;

  PROCEDURE SEARCHRACKS(
    p_BatchField1 inv_container_batches.batch_field_1%TYPE,
    p_BatchField2 inv_container_batches.batch_field_2%TYPE,
    p_BatchField3 inv_container_batches.batch_field_3%TYPE,
    p_OpenPositions NUMBER,
    p_ContainerSize inv_containers.qty_max%TYPE,
    p_RestrictSize VARCHAR2,
    p_ContainerSizeUOM inv_containers.unit_of_meas_id_fk%TYPE,
    O_RS OUT CURSOR_TYPE
  )  AS
  my_sql varchar2(3000) ;
  bAndPredicate boolean ; 
  l_Predicate varchar2(10) ;
  l_SizeCompare varchar2(10) ;
  BEGIN
    bAndPredicate := false ;
    my_sql := 'select 
       Racks.NUMBEROFOPENGRIDS2(inv_locations.location_id) as OpenPositions
       , Racks.NUMBEROFGRIDPOSITIONS(inv_locations.location_id)-Racks.NUMBEROFOPENGRIDS2(inv_locations.location_id) as FilledPositions
       , GUIUTILS.GETRACKLOCATIONPATH(inv_locations.location_id) as LocationPath
       , GUIUTILS.GETFULLRACKLOCATIONPATH(inv_locations.location_id) as LocationPathFull
       , location_id
       , location_name 
	   , (select gl.location_id || ''::'' || gl.name
		  from inv_vw_grid_location gl
		 where gl.parent_id = inv_locations.location_id
		   and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */
				 location_id_fk
				  from inv_containers c
				 where c.location_id_fk = gl.location_id)
		   and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */
				 location_id_fk
				  from inv_plates p
				 where p.location_id_fk = gl.location_id)
		   and not exists (select /*+ index(lr INV_LOCATION_PK) */
				 parent_id
				  from inv_locations l
				 where l.parent_id = gl.location_id
				   and collapse_child_nodes = 1)
		   and rownum = 1) as FirstOpenPosition
       from inv_locations where ' ;
    if p_ContainerSize is not null then
       if p_RestrictSize = 'on' then
          l_SizeCompare := '=' ;
       else
          l_SizeCompare := '>=' ;
       end if ;
       my_sql := my_sql || ' location_id in (
          select distinct il.location_id from inv_vw_grid_location_lite igll, inv_locations il
            where igll.parent_id = il.location_id
            and il.collapse_child_nodes = 1
            and igll.location_id in (select distinct location_id_fk from inv_containers where qty_max ' || '=' || ' ' || p_ContainerSize || ')
        )' ; 
       bAndPredicate := true ;
    end if ; 
    if p_BatchField1 is not null or p_BatchField2 is not null or p_BatchField3 is not null then
       if bAndPredicate = false then
         l_Predicate := '' ;
       else
         l_Predicate := 'and' ;
       end if ;
       my_sql := my_sql || l_Predicate || ' location_id in (
          select distinct il.location_id from inv_vw_grid_location_lite igll, inv_locations il
            where igll.parent_id = il.location_id
            and il.collapse_child_nodes = 1
            and igll.location_id in (select distinct ic.location_id_fk from inv_containers ic, inv_container_batches icb, inv_vw_reg_batches ivrb
              where icb.batch_id = ic.batch_id_fk
              and icb.batch_field_1 = ivrb.RegID
              and icb.batch_field_2 = ivrb.BatchNumber
              and ivrb.RegNumber like ''%' || p_BatchField1 || '%''
              and ivrb.BatchNumber like ''%' || p_BatchField2 || '%'')
        )' ;
       bAndPredicate := true ;
    end if ;
    if p_ContainerSizeUOM is not null then
       if bAndPredicate = false then
         l_Predicate := '' ;
       else
         l_Predicate := 'and' ;
       end if ;
       my_sql := my_sql || l_Predicate || ' location_id in (
          select distinct il.location_id from inv_vw_grid_location_lite igll, inv_locations il
            where igll.parent_id = il.location_id
            and il.collapse_child_nodes = 1
            and igll.location_id in (select distinct location_id_fk from inv_containers where unit_of_meas_id_fk ' || '=' || ' ' || p_ContainerSizeUOM || ')
        )' ;
       bAndPredicate := true ;
    end if ;
    if p_OpenPositions is not null then
       if bAndPredicate = false then
         l_Predicate := '' ;
       else
         l_Predicate := 'and' ;
       end if ;
       my_sql := my_sql || l_Predicate || ' Racks.NUMBEROFOPENGRIDS2(inv_locations.location_id) >= ' || p_OpenPositions ;
       bAndPredicate := true ;
    end if ;
    my_sql := my_sql || ' order by location_name' ;

    OPEN O_RS FOR
    my_sql ;
  
  END SEARCHRACKS;



  FUNCTION NUMBEROFCONTAINERSINGRID(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2
  IS
    l_count VARCHAR2(10);
  BEGIN

    select count(*) into l_count from inv_containers
    where location_id_fk in (select location_id
    from inv_vw_grid_location
    where parent_id = p_LocationID);

    RETURN l_count;

  END NUMBEROFCONTAINERSINGRID;


  FUNCTION NUMBEROFPLATESINGRID(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2
  IS
    l_count VARCHAR2(10);
  BEGIN

    select count(*) into l_count from inv_plates
    where location_id_fk in (select location_id
    from inv_vw_grid_location
    where parent_id = p_LocationID);

    RETURN l_count;

  END NUMBEROFPLATESINGRID;

  FUNCTION NUMBEROFRACKSINGRID(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2
  IS
    l_count VARCHAR2(10);
  BEGIN

    select count(*) into l_count
    from inv_locations
    where parent_id in (
       select location_id
       from inv_vw_grid_location
       where parent_id = p_LocationID);

    RETURN l_count;

  END NUMBEROFRACKSINGRID;


  FUNCTION NUMBEROFOPENGRIDS(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN NUMBER
  IS
    l_count NUMBER(4);
  BEGIN

      select ((f.row_count * f.col_count) -
              ((select count(*)
                   from inv_containers
                  where location_id_fk in
                        (select location_id
                           from inv_vw_grid_location
                          where parent_id = l.location_id)) +
              (select count(*)
                   from inv_plates
                  where location_id_fk in
                        (select location_id
                           from inv_vw_grid_location
                          where parent_id = l.location_id)) +
              (select count(*)
                   from inv_locations
                  where parent_id in
                        (select location_id
                           from inv_vw_grid_location
                          where parent_id = l.location_id)))) into l_count
      from inv_grid_storage s, inv_locations l, inv_grid_format f
      where s.location_id_fk = l.location_id
      and s.grid_format_id_fk = f.grid_format_id
      and l.location_id = p_LocationID;

    RETURN l_count;

  END NUMBEROFOPENGRIDS;


  FUNCTION NUMBEROFOPENGRIDS2(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN NUMBER
  IS
    l_count NUMBER(4);
    l_numGridPositions NUMBER(4);
    l_numContainers number(4);
    l_numPlates number(4);
    l_numRacks number(4);
  BEGIN
    l_numGridPositions := NUMBEROFGRIDPOSITIONS(p_LocationID);
    select count(*) into l_numContainers
      from inv_containers
      where location_id_fk in
      (select location_id
      from inv_vw_grid_location
      where parent_id = p_LocationID);

      select count(*) into l_numPlates
        from inv_plates
        where location_id_fk in
        (select location_id
        from inv_vw_grid_location
        where parent_id = p_LocationID);

      select count(*) into l_numRacks
        from inv_locations
        where parent_id in
        (select location_id
        from inv_vw_grid_location
        where parent_id = p_LocationID);
      l_count := l_numGridPositions - l_numContainers - l_numPlates - l_numRacks;
      if l_count = '' or l_count is null then
         l_count := 0;
      end if;
                 
    RETURN l_count;

  END NUMBEROFOPENGRIDS2;



  FUNCTION NUMBEROFGRIDPOSITIONS(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN NUMBER
  IS
    l_count VARCHAR2(10);
  BEGIN

    select row_count*col_count into l_count from inv_grid_storage igs, inv_grid_format igf
    where igs.grid_format_id_fk = igf.grid_format_id 
    and igs.location_id_fk = p_LocationiD;

    RETURN l_count;

  END NUMBEROFGRIDPOSITIONS;


  FUNCTION GETDEFAULTGRIDLOCATION(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2
  IS
    l_value VARCHAR2(10);
  BEGIN

      select min(location_id || '::' || name) into l_value
           from inv_vw_grid_location
          where parent_id = p_LocationID
            and location_id not in
                (select distinct location_id_fk from inv_containers)
            and location_id not in
                (select distinct location_id_fk from inv_plates)
            and location_id not in
                (select distinct parent_id
                   from inv_locations
                  where collapse_child_nodes = 1);

    RETURN l_value;

  END GETDEFAULTGRIDLOCATION;


  PROCEDURE REPORTINVALIDGRIDS(
    p_LocationID inv_locations.location_id%TYPE,
    O_RS OUT CURSOR_TYPE
    ) AS
    BEGIN

		OPEN O_RS FOR

        select c.container_id as container_id, c.location_id_fk as location_id_fk, vl.name as name
        from inv_containers c, inv_vw_grid_location vl
        where c.location_id_fk = vl.location_id
        and c.location_id_fk in (
          select location_id_fk from 
          inv_containers 
          where location_id_fk in (
            select location_id from inv_vw_grid_location
            where parent_id=p_LocationID
          )
          having count(*) > 1
          group by location_id_fk
        );

  END REPORTINVALIDGRIDS;


END RACKS;


/

show errors;