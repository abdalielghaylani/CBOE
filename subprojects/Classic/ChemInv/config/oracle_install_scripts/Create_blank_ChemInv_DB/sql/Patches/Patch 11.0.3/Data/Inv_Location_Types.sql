insert into INV_LOCATION_TYPES (LOCATION_TYPE_ID, LOCATION_TYPE_NAME, GRAPHIC_ID_FK)
(select null, 'Public', GRAPHIC_ID from INV_GRAPHICS where URL_ACTIVE='public_icon.gif');
