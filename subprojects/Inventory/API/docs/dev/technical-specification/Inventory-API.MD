PerkinElmer.COE.Inventory.API
=============================
**Version:** v1

### /api/v{version}/containers/byId/{id}
---
##### ***GET***
**Summary:** Get container by internal id

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Internal id | Yes | integer |
| version | path |  | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | OK | [ContainerData](#containerdata) |
| 400 | BadRequest | object |
| 401 | Unauthorized | object |
| 500 | InternalServerError | object |

### /api/v{version}/containers/{containerId}
---
##### ***GET***
**Summary:** Get container by container id

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| containerId | path | Container id | Yes | string |
| version | path |  | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | OK | [ContainerData](#containerdata) |
| 400 | BadRequest | object |
| 401 | Unauthorized | object |
| 500 | InternalServerError | object |

### /api/v{version}/locations
---
##### ***GET***
**Summary:** Get all locations

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| version | path |  | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | OK | [ [LocationData](#locationdata) ] |
| 400 | BadRequest | object |
| 401 | Unauthorized | object |
| 500 | InternalServerError | object |

### /api/v{version}/locations/{id}
---
##### ***GET***
**Summary:** Get location by id

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Location id | Yes | integer |
| version | path |  | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | OK | [LocationData](#locationdata) |
| 400 | BadRequest | object |
| 401 | Unauthorized | object |
| 500 | InternalServerError | object |

### Models
---

### ContainerData  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ID | integer | Gets or sets the internal identifier of the container. | No |
| Container ID | string | Gets or sets the container id of the container. | No |
| Name | string | Gets or sets the name of the container. | No |
| Location | string | Gets or sets the location of the container. | No |
| Supplier | string | Gets or sets the supplier of the container. | No |
| Current User | string | Gets or sets the current user. | No |
| Container Size | string | Gets or sets the size of the container. | No |
| Date Created | dateTime | Gets or sets the date created. | No |
| Container Type | string | Gets or sets the type of the container. | No |
| Status | string | Gets or sets the status of the container. | No |
| Quantity Available | string | Gets or sets the qualtity available. | No |

### LocationData  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ID | integer | Gets or sets the internal identifier of the location. | No |
| Parent ID | integer | Gets or sets the internal identifier of the parent location. | No |
| Name | string | Gets or sets the name of the location. | No |
| Description | string | Gets or sets the description of the location. | No |
| Type | string | Gets or sets the type of the location. | No |