insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-101, 'A container with the same name already exists at this location.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-102, 'A container with same barcode ID already exists.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-103, 'Amount cannot exceed container size.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-104, 'Could not find the destination location.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-105, 'A location with same barcode ID already exists.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-105, 'A location with same barcode ID already exists.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-106, 'A location with same name already exists at this location.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-107, 'Could not find the container to delete.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-108, 'Cannot delete location because it is not empty or it''s part of a grid.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-109, 'Could not find the container to be moved.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-110, 'This is a system location that cannot be updated or deleted.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-113, 'The root location cannot be moved.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-114, 'Location cannot be moved onto itself or onto one of its sublocations.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-115, 'This is a system location that cannot be moved.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-119, 'Could not find the container to be updated.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-122, 'Cannot reserve more than total quantity available.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-123, 'Cannot create multiple copies of a container using a non-numerical barcode id.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-124, 'Cannot update or delete physical plate because existing plate formats refer to it.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-125, 'Cannot update or delete  storage format because existing locations refer to it.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-126, 'Cannot update or delete  plate format because existing plates refer to it.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-127, 'Cannot update or delete well format because exisiting wells refer to it. ');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-128, 'Container of this type not allowed at this location.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-129, 'Cannot delete substance because it''s  referenced by one or more containers.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-130, 'Invalid column specified.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-131, 'Number of plates exceeds number of empty locations.');
insert into INV_API_ERRORS (ERROR_ID, ERROR_TEXT)
values (-132, 'Cannot update or delete plate type because existing plates refer to it.');
commit;
