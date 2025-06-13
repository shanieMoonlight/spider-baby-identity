# Pagination, Filtering, and Sorting API Guide (Frontend)

This API lets you request paged, filtered, and sorted data from the backend using a single JSON object. Here’s how to build your request and what each part means.

---

## Example Request

```json
{
  "pageNumber": 0,
  "pageSize": 20,
  "filterList": [
    {
      "field": "name",
      "filterType": "contains",
      "filterValue": "john",
      "filterValues": [],
      "filterDataType": "string"
    }
  ],
  "sortList": [
    {
      "field": "createdAt",
      "sortDescending": true
    }
  ]
}
```

---

## Request Structure

### `pageNumber` (number)
- The page of results you want (**1-based**: 1 = first page).

### `pageSize` (number)
- How many results per page.

### `filterList` (array of FilterRequest)
- Each object describes a filter to apply.

#### FilterRequest fields:
| Field           | Type      | Description                                                                                 |
|-----------------|-----------|---------------------------------------------------------------------------------------------|
| `field`         | string    | The property/column name to filter on (e.g. `"name"`, `"email"`, `"createdAt"`).            |
| `filterType`    | string    | The type of filter (see below for options).                                                 |
| `filterValue`   | string    | The value to compare against (e.g. `"john"` for name contains john).                        |
| `filterValues`  | string[]  | Used for "where in" filters (e.g. list of IDs).                                             |
| `filterDataType`| string    | The data type (`"string"`, `"number"`, `"date"`, `"boolean"`).                             |

##### Common `filterType` values:
- `"equals"`: Exact match
- `"not_equal_to"`: Not equal
- `"contains"`: String contains value
- `"starts_with"`: String starts with value
- `"ends_with"`: String ends with value
- `"greater_than"` / `"less_than"`: For numbers/dates
- `"between"`: For ranges (numbers/dates)
- `"in"`: Value is in a list (`filterValues`)

See the full list in the backend docs or ask your backend dev for all options.

##### Example filters:
- Get users whose name contains "john":
  ```json
  { "field": "name", "filterType": "contains", "filterValue": "john", "filterDataType": "string" }
  ```
- Get users with IDs in a list:
  ```json
  { "field": "id", "filterType": "in", "filterValues": ["1", "2", "3"], "filterDataType": "number" }
  ```

---

### `sortList` (array of SortRequest)
- Each object describes a sort order.

#### SortRequest fields:
| Field           | Type      | Description                                                                                 |
|-----------------|-----------|---------------------------------------------------------------------------------------------|
| `field`         | string    | The property/column name to sort by (e.g. `"name"`, `"createdAt"`).                         |
| `sortDescending`| boolean   | `true` for descending order, `false` for ascending.                                         |

##### Example sorts:
- Sort by newest first:
  ```json
  { "field": "createdAt", "sortDescending": true }
  ```
- Sort by name A-Z:
  ```json
  { "field": "name", "sortDescending": false }
  ```

---

## How to Use

1. Build your request object as shown above.
2. Send it as JSON in your API call (usually as the POST body).
3. The backend will return a paged, filtered, and sorted list based on your criteria.

---

## Filter Types Reference

| Filter Type                | Description                                 | Data Types Supported   |
|---------------------------|---------------------------------------------|-----------------------|
| `equals`                  | Exact match                                 | string, number, date, boolean |
| `not_equal_to`            | Not equal                                   | string, number, date, boolean |
| `contains`                | String contains value                       | string                |
| `starts_with`             | String starts with value                    | string                |
| `ends_with`               | String ends with value                      | string                |
| `greater_than`            | Greater than                                | number, date          |
| `greater_than_or_equal_to`| Greater than or equal to                    | number, date          |
| `less_than`               | Less than                                   | number, date          |
| `less_than_or_equal_to`   | Less than or equal to                       | number, date          |
| `between`                 | Between two values (range)                  | number, date          |
| `in`                      | Value is in a list (`filterValues`)         | string, number, date  |

---

For more details, see backend documentation or ask your backend developer.
