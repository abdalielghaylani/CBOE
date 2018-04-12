CREATE OR REPLACE
PACKAGE Constants AS

  -- Exceptions
  e_QTimeOut                     EXCEPTION;
  PRAGMA EXCEPTION_INIT(e_QTimeOut, -25228);
  e_QOutOfSequence               EXCEPTION;
  PRAGMA EXCEPTION_INIT(e_QOutOfSequence, -25237);
  e_UserCancel                   EXCEPTION;
  PRAGMA EXCEPTION_INIT(e_UserCancel, -01013);
  e_UniqueConstraint             EXCEPTION;
  PRAGMA EXCEPTION_INIT(e_UniqueConstraint, -00001);

  -- System Parameters
  cPurchasingEmailAddress        CONSTANT NUMBER := 1;
  cSenderEmailAddress            CONSTANT NUMBER := 2;
  cMailServer                    CONSTANT NUMBER := 3;
  cEHSEmailAddress               CONSTANT NUMBER := 4;
  cCIAEmailAddress               CONSTANT NUMBER := 5;
  cReceivingEmailAddress         CONSTANT NUMBER := 6;
  cPurchasingUser                CONSTANT NUMBER := 7;
  cCIAUser                       CONSTANT NUMBER := 8;

  -- Container_Order.order_source values
  cOrderSourceNewSubstance       CONSTANT NUMBER := 1;
  cOrderSourceReorder            CONSTANT NUMBER := 2;
  cOrderSourceACX                CONSTANT NUMBER := 3;

--  vDummyParameter                System_Parameter.name%TYPE;
--  vDummyStringParameter          System_Parameter.stringValue%TYPE;

  -- constants for container status
  cAvailable                     CONSTANT NUMBER := 1;
  cEmpty                         CONSTANT NUMBER := 2;
  cOrderPending                  CONSTANT NUMBER := 3;
  cOrdered                       CONSTANT NUMBER := 4;
  cInTransit                     CONSTANT NUMBER := 5;
  cDisposed                      CONSTANT NUMBER := 6;
  cMissing                       CONSTANT NUMBER := 7;
  cUnknown                       CONSTANT NUMBER := 0;
  cCanceled                      CONSTANT NUMBER := 8;
  cInUse                         CONSTANT NUMBER := 9;
  cOrderItemSubmitted            CONSTANT NUMBER := 10;
  cOrderItemWRegistrationError   CONSTANT NUMBER := 11;
  cBackorderedItem               CONSTANT NUMBER := 12;
  cDiscontinuedItem              CONSTANT NUMBER := 13;
  cRecognizedAtDock              CONSTANT NUMBER := 14;
  cReceived                      CONSTANT NUMBER := 15;
  cRequested                     CONSTANT NUMBER := 16;
  cNotEnoughAvailable            CONSTANT NUMBER := 17;
  cItemFound                     CONSTANT NUMBER := 18;
  cToBeReturned                  CONSTANT NUMBER := 19;
  cReturned                      CONSTANT NUMBER := 20;
  cMovedDuringReconcileLoc       CONSTANT NUMBER := 21;
  cMissingDuringReconcileLoc     CONSTANT NUMBER := 22;

  -- constants for special locations
  cRootLoc                       CONSTANT NUMBER := 0;
  cOnOrderLoc                    CONSTANT NUMBER := 1;
  cDisposedLoc                   CONSTANT NUMBER := 2;
  cTrashCanLoc                   CONSTANT NUMBER := 3;
  cMissingLoc                    CONSTANT NUMBER := 4;

  -- constants for location types
  cUnknown                       CONSTANT NUMBER := 0;
  cCompany                       CONSTANT NUMBER := 1;
  cSite                          CONSTANT NUMBER := 2;
  cBuilding                      CONSTANT NUMBER := 3;
  cStockRoom                     CONSTANT NUMBER := 4;
  cLaboratory                    CONSTANT NUMBER := 5;
  cBench                         CONSTANT NUMBER := 6;
  cHood                          CONSTANT NUMBER := 7;
  cDryBox                        CONSTANT NUMBER := 8;
  cRefrigerator                  CONSTANT NUMBER := 9;
  cFreezer                       CONSTANT NUMBER := 10;
  cShelf                         CONSTANT NUMBER := 11;
  cBin                           CONSTANT NUMBER := 12;
  cPan                           CONSTANT NUMBER := 13;
  cSolventCabinet                CONSTANT NUMBER := 14;
  cSolventRoom                   CONSTANT NUMBER := 15;
  cCabinet                       CONSTANT NUMBER := 16;
  cGMP                           CONSTANT NUMBER := 17;
  cInstrumentRoom                CONSTANT NUMBER := 18;
  cCylinderStorage               CONSTANT NUMBER := 19;
  cReceiving                     CONSTANT NUMBER := 20;
  cBox                           CONSTANT NUMBER := 21;
  cUtilityRoom                   CONSTANT NUMBER := 22;
  cDisposalNeutralizationRoom    CONSTANT NUMBER := 23;
  cSafe                          CONSTANT NUMBER := 24;
  cUltraFreezer                  CONSTANT NUMBER := 25;
  cPlateMap                      CONSTANT NUMBER := 26;
  cRack                          CONSTANT NUMBER := 27;
  cDisposedLocType               CONSTANT NUMBER := 500;
  cOnOrderLocType                CONSTANT NUMBER := 501;
  cTrashCanLocType               CONSTANT NUMBER := 502;
  cStockRoomShelf                CONSTANT NUMBER := 503;
  cStockRoomCabinet              CONSTANT NUMBER := 504;
  cStockRoomBin                  CONSTANT NUMBER := 505;
  cMissingLoc                    CONSTANT NUMBER := 506;
  cStockRoomRefrigerator         CONSTANT NUMBER := 507;
  cStockRoomTopOfCabinet         CONSTANT NUMBER := 508;
  cStockRoomFreezer              CONSTANT NUMBER := 509;
  cStockRoomReturnsRoom          CONSTANT NUMBER := 510;

  -- constants for barcodes
  cPlateBarcodeDesc		        CONSTANT NUMBER := 1;
  cLocationBarcodeDesc			CONSTANT NUMBER := 2;
  cContainerBarcodeDesc 		CONSTANT NUMBER := 3;

  -- true and false
  cTRUE                          CONSTANT INTEGER(1) := 1;
  cFALSE                         CONSTANT INTEGER(1) := 0;

  -- INVADMIN user name
  cInvAdmin                      CONSTANT VARCHAR2(8) := 'INVADMIN';

  -- the types of email messages.
  cEmailOrderSingleItemType      CONSTANT INTEGER(1) := 1;
  cEmailOrderInfoType            CONSTANT INTEGER(1) := 2;
  cEmailMovedCarcinogenType      CONSTANT INTEGER(1) := 3;
  cEmailNotEnoughAvailableType   CONSTANT INTEGER(1) := 4;

  -- the suppliers
  cUnknownSupplier               CONSTANT NUMBER := 0;
  cNewSupplier                   CONSTANT NUMBER := 1000;

  -- order separators
  cNewSupplierSeparator CONSTANT VARCHAR2(60) := '%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%';
  cSupplierSeparator    CONSTANT VARCHAR2(60) := '************************************************************';
  cOrderItemSeparator   CONSTANT VARCHAR2(60) := '------------------------------------------------------------';

  -- units constants
  cPoundsUOM                     CONSTANT NUMBER := 11;
  cCubicFeetUOM                  CONSTANT NUMBER := 18;

  -- CAS translation strings
  cCASTranslation1     CONSTANT VARCHAR2(70) := '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ -~!@#$%^&*()_+={}[];:,./<>?|';
  cCASTranslation2     CONSTANT VARCHAR2(10) := '0123456789';

  -- Catalog Number translation strings
  cCatNumTranslation1     CONSTANT VARCHAR2(70) := '0123456789 -~!@#$%^&*()_+={}[];:,./<>?|';
  cCatNumTranslation2     CONSTANT VARCHAR2(10) := '0123456789';

  -- Units
  cMoleID			CONSTANT NUMBER := 52;
  cLiterID			CONSTANT NUMBER := 2;
  cMolarID			CONSTANT NUMBER := 15;
  cGramID			CONSTANT NUMBER := 5;  
  cMicroLiterID 	CONSTANT NUMBER := 4;
  
  -- Unit Types
  cVolumeID			CONSTANT NUMBER := 1;
  cMassID			CONSTANT NUMBER := 2;
  -- Constants for plate status
  cUnknown_pl					CONSTANT NUMBER := 5;
  cTested_pl                    CONSTANT NUMBER := 6;
  cUntested_pl					CONSTANT NUMBER := 7;
  cDestroyed_pl					CONSTANT NUMBER := 8;
  -- Batch Grouping Constants
  cContainerBatchField1          CONSTANT VARCHAR(50) := 'REG_ID_FK';
  cContainerBatchField1Display   CONSTANT VARCHAR(50) := 'A-Code';
  cContainerBatchField2          CONSTANT VARCHAR(50) := 'BATCH_NUMBER_FK';
  cContainerBatchField2Display   CONSTANT VARCHAR(50) := 'Lot Number';
  cContainerBatchField3          CONSTANT VARCHAR(50) := '';
  cContainerBatchField3Display   CONSTANT VARCHAR(50) := '';
  -- constants for request status
  cUnknownRequest                CONSTANT NUMBER := 1;
  cNewRequest                    CONSTANT NUMBER := 2;
  cApprovedRequest               CONSTANT NUMBER := 3;
  cDeclinedRequest               CONSTANT NUMBER := 4;
  cFilledRequest                 CONSTANT NUMBER := 5;
  cClosedRequest                 CONSTANT NUMBER := 6;
  cCancelledRequest              CONSTANT NUMBER := 7;
  cPendingRequest                CONSTANT NUMBER := 8;
END;
/
show errors;
