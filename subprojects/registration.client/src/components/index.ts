import { RegAlert } from './alert';
import { RegButton } from './button';
import { RegContainer } from './container';
import { RegLogo } from './logo';
import { RegLoginModal, RegLoginForm } from './login';
import { RegNavigator, RegNavigatorHeader, RegNavigatorItems, RegNavigatorItem } from './navigator';
import { RegModal, RegModalContent } from './modal';
import { RegRecords, RegRecordDetail, RegRecordSearch, RegQueryManagement, RegTemplates } from './registry';
import { IResponseData, IRecordSaveData, ITemplateData, CTemplateData } from './registry';
import { RegStructureImage } from './structure-image';
import {
  RegForm,
  RegFormError,
  RegFormGroup,
  RegLabel,
  RegInput,
  RegInputGroup,
  RegInputLogin
} from './form';
import { RegFooter } from './footer';
import { RegSidebar } from './navigator/sidebar.component';
import { RegPageHeader, RegSettingsPageHeader } from './page-header';

export * from './configuration';
export {
  RegAlert,
  RegButton,
  RegContainer,
  RegInput,
  RegInputLogin,
  RegInputGroup,
  RegLogo,
  RegLoginModal,
  RegLoginForm,
  RegForm, RegFormError, RegFormGroup,
  RegRecords,
  RegQueryManagement,
  RegTemplates,
  RegRecordSearch,
  RegRecordDetail,
  RegStructureImage,
  RegLabel,
  RegModal, RegModalContent,
  RegNavigator, RegNavigatorHeader, RegNavigatorItems, RegNavigatorItem,
  RegSidebar, RegFooter, RegPageHeader, RegSettingsPageHeader,
  IResponseData, IRecordSaveData, ITemplateData, CTemplateData
};
