Please follow the conventions described in this document.

The "Xslt" folder
=-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-=
Contains XSLTs used to:
(1) convert RegistryRecord domain xml into Oracle DB-friendly xml for CRUD operations, and
(2) convert the logical DB content of a Registration, plus validation rules and Add-Ins, to
    xml that is used to hydrate the RegistryRecord domain object.

All other XSLT processes that, at the time of writing this (17-FEB-2011, Jeff Dugas),
are embedded in the DB but need to be tested outside of the DB should be added to the XSLT folder.

The "CSBR" folder
=-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-=
Contains all bug-related tests.
Required test materials should be added to a Content subfolder or, preferably, embedded in the test
class directly to ensure upkeep with the test itself.

All fixed CSBRs, and those which are rejected, must have a matching test fixture with a robust
test-set within the single fixture.

The "Helpers" folder
=-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-==-=
Contains utility classes, constants, etc., generally-applicable to all or most test fixtures.
As the test fixtures mature, please be sure to move repeat-use functions, constants, etc., into
classes in the Helpers folder, INCLUDING CODE-DOCUMENTATION explaining their existence.