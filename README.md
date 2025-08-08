# Project Title: Large-Scale E-Commerce Sales Analytics (MySQL Edition)

## **Premise**

Create an end-to-end application that:

1. Generates a **large CSV** file of synthetic e-commerce sales data.
2. **Uploads** this CSV data into a MySQL database via the application.
3. **Stores** the data in MySQL tables optimized for large-scale queries (e.g., proper indexing).
4. Provides ways to **download** data or aggregated reports from the application (via CSV or other formats).
5. Offers analytics endpoints (e.g., total sales over time, top products) for your front end app.

This project focuses on **data efficiency**: handling millions of records, ensuring queries remain performant, and demonstrating how to load and retrieve data at scale using MySQL.

---

## **High-Level Requirements**

1. **Data Generation**  
   - A Node.js script that creates a **large synthetic dataset** simulating realistic orders (user IDs, products, countries, etc.).  
   - The script outputs a CSV file with columns such as:
     - `order_id`, `order_date`, `user_id`, `product_id`, `quantity`, `price`, `total_amount`, `country`, `city`.

2. **Data Ingestion (MySQL)**  
   - A table schema (e.g., `orders`) stored in MySQL, with indexes on key columns (like `order_date` or `product_id`) to handle large volumes efficiently.  
   - A process (called from the frontend app) that **uploads the CSV** into MySQL.

3. **Front-End / App Integration**  
   - Users can **upload** the large CSV file through the app.  
   - The app provides analytics endpoints to report on:
     - Summarize total sales per day, top-selling products, busiest regions, etc.
   - The app can **export** data subsets or aggregated stats as CSV.

4. **Data Efficiency Considerations**  
   - Use appropriate **indexes** to improve query performance on large data sets.  
   - Consider advanced MySQL features like **partitioning** on `order_date` if dealing with millions of rows.
   - For high-traffic scenarios, you can implement **caching** in your backend.

5. **Deployment & Workflow**  
   - Provide instructions on how to build/run the application locally.  

---

Below is an example Node.js script that generates a large CSV file with random e-commerce orders. 
By default, it creates a file named `synthetic_orders.csv` with 1,000,000 rows, but you can adjust the 'numRecords' parameter value as needed. 
In the final app, we want you to report on 5-10 million rows of 'orders' data. 
If your computer's resources are limited, stick to 1,000,000 rows or less.

```js
// Give this file the name 'generateSyntheticEcommerceData.js'
/**
 * Generates a large CSV file with random e-commerce orders for testing data-intensive applications.
 * Usage (from terminal): node generateSyntheticEcommerceData.js
 */

const fs = require("fs");
const { v4: uuidv4 } = require("uuid");

// Define some sample products, user pools, and geolocations
const products = [
  { product_id: 1001, name: "Wireless Mouse", base_price: 19.99 },
  { product_id: 1002, name: "Bluetooth Headphones", base_price: 49.99 },
  { product_id: 1003, name: "Mechanical Keyboard", base_price: 89.99 },
  { product_id: 1004, name: "USB-C Cable", base_price: 9.99 },
  { product_id: 1005, name: "Webcam", base_price: 39.99 },
  { product_id: 1006, name: "Laptop Stand", base_price: 25.99 },
  { product_id: 1007, name: "4K Monitor", base_price: 229.99 },
  { product_id: 1008, name: "Portable SSD", base_price: 79.99 },
  { product_id: 1009, name: "Smartphone Case", base_price: 15.99 },
  { product_id: 1010, name: "Wireless Charger", base_price: 29.99 },
];

const countries = ["USA", "Canada", "UK", "Germany", "France", "Australia", "Japan", "Brazil", "India", "Mexico"];

const cities = {
  USA: ["New York", "Los Angeles", "Chicago", "Houston", "Seattle"],
  Canada: ["Toronto", "Vancouver", "Montreal", "Ottawa"],
  UK: ["London", "Manchester", "Bristol", "Birmingham"],
  Germany: ["Berlin", "Munich", "Hamburg", "Frankfurt"],
  France: ["Paris", "Lyon", "Marseille", "Toulouse"],
  Australia: ["Sydney", "Melbourne", "Brisbane", "Perth"],
  Japan: ["Tokyo", "Osaka", "Yokohama", "Nagoya"],
  Brazil: ["Sao Paulo", "Rio de Janeiro", "Brasilia", "Salvador"],
  India: ["Mumbai", "Delhi", "Bangalore", "Hyderabad"],
  Mexico: ["Mexico City", "Guadalajara", "Monterrey", "Puebla"],
};

function generateSyntheticEcommerceData(outputCsv = "synthetic_orders.csv", numRecords = 1_000_000) {
  // Determine date range (last 365 days up to today)
  const now = new Date();
  const startDate = new Date(now.getTime() - 365 * 24 * 60 * 60 * 1000); // 365 days ago

  // Create a writable stream for CSV
  const writeStream = fs.createWriteStream(outputCsv, { encoding: "utf-8" });

  // Write header row
  writeStream.write([
    "order_id",
    "order_date",
    "user_id",
    "product_id",
    "quantity",
    "price",
    "total_amount",
    "country",
    "city"
  ].join(",") + "\n");

  for (let i = 0; i < numRecords; i++) {
    const orderId = uuidv4();

    // Generate a random date within the last 365 days
    const randomDays = Math.floor(Math.random() * 365);
    const orderDate = new Date(startDate.getTime() + randomDays * 24 * 60 * 60 * 1000);
    const orderDateStr = orderDate.toISOString().split("T")[0]; // YYYY-MM-DD

    // Random user ID
    const userId = Math.floor(Math.random() * 900_000) + 100_000; // 100000-999999

    // Random product
    const product = products[Math.floor(Math.random() * products.length)];
    const productId = product.product_id;
    const basePrice = product.base_price;

    // Random quantity and slight price variation
    const quantity = Math.floor(Math.random() * 5) + 1; // 1-5
    const finalPrice = +(basePrice * (0.9 + Math.random() * 0.2)).toFixed(2); // +/- 10%
    const totalAmount = +(finalPrice * quantity).toFixed(2);

    // Random country and city
    const country = countries[Math.floor(Math.random() * countries.length)];
    const cityList = cities[country];
    const city = cityList[Math.floor(Math.random() * cityList.length)];

    // Write one CSV record
    writeStream.write([
      orderId,
      orderDateStr,
      userId,
      productId,
      quantity,
      finalPrice,
      totalAmount,
      country,
      city
    ].join(",") + "\n");
  }

  // End the stream
  writeStream.end(() => {
    console.log(`Successfully generated ${numRecords} synthetic records in '${outputCsv}'.`);
  });
}

// Run the script if called directly (e.g., `node generateSyntheticEcommerceData.js`)
if (require.main === module) {
  generateSyntheticEcommerceData("synthetic_orders.csv", 100000);
}

// Otherwise, you can export the function for use in other modules:
// module.exports = { generateSyntheticEcommerceData };
```

### How to Use

1. **Install Dependencies** (only `uuid` is required here; everything else is native Node.js):
   ```bash
   npm install uuid
   ```
   
2. **Run the Script**:
   ```bash
   node generateSyntheticEcommerceData.js
   ```
   This will create a file named `synthetic_orders.csv` with 1,000,000 synthetic e-commerce order records.

3. **Adjust Parameters**:  
   - To change the filename or the number of records, modify the function call inside `if (require.main === module)` or update them manually:
     ```bash
     node generateSyntheticEcommerceData.js
     ```
     (You can pass arguments to your script or edit the function call directly in the file.)

4. **Output**:  
   - A CSV file with columns:  
     ```
     order_id,order_date,user_id,product_id,quantity,price,total_amount,country,city
     ```
   - Each row is a random order that mimics an e-commerce purchase event.

---

## **Example Bash Command for MySQL Data Load**

After generating your CSV file, you can load it into MySQL using a command (from terminal or script) such as:

```bash
mysql -u your_user -p your_database -e "
LOAD DATA LOCAL INFILE '/path/to/synthetic_orders.csv'
INTO TABLE orders
FIELDS TERMINATED BY ','
ENCLOSED BY '\"'
LINES TERMINATED BY '\n'
IGNORE 1 ROWS
(order_id, order_date, user_id, product_id, quantity, price, total_amount, country, city);
"
```

> **Note**:  
> - You’ll need a matching table schema in MySQL that corresponds to the CSV columns (e.g., `orders`).  
> - Ensure `secure_file_priv` or local file loading is allowed in your MySQL configuration (depending on your environment).  
> - NOTE - This is so you can test your analytic endpoints before the frontend 'upload csv' feature is finished. The final app will require the ability to upload a csv file, with potentially millions of rows of data, through the app's frontend.

---

## **Summary of Workflow**

1. **Generate Data**  
   - Run the Node.js script to produce a large CSV of synthetic orders (e.g., `synthetic_orders.csv`).

2. **Create MySQL Schema**  
   - Define your `orders` table (and any supporting tables) with the appropriate columns/indexes.

3. **Upload via App**  
   - Use a front end form/app endpoint to upload and parse the CSV into MySQL.  

4. **Data Retrieval / Download**  
   - Provide endpoints to download subsets of the data as CSV or to retrieve summary metrics (e.g., daily sales totals, top products) for reporting.

5. **Indexing & Performance**  
   - If data volumes are large (millions of rows), add indexes on columns commonly used in filters/groupings (e.g., `order_date`, `product_id`, `country`).  
   - Consider partitioning by date if the dataset grows extremely large.

6. **Visualizations & Reporting**  
   - In the Frontend app, Display dashboards with sales analytics (time-series charts, top products, etc.). [You can use a library like Recharts to report on metrics like 'top products over time'.](https://recharts.org/en-US/)
   - Allow exports of aggregated data from the app.

---

## **Conclusion**

This **MySQL-based** version of the E-Commerce Sales Analytics project offers a real-world scenario of handling and analyzing large-scale data. By focusing on:

- **Efficient data pipelines in your backend**,
- **Proper indexing and table design**, and
- **Uploading/downloading** data within your application,

you’ll gain valuable experience in data engineering, performance tuning, and full-stack development centered around MySQL as your primary data store. 

Good Luck!