# Mega Account Manager

Mega Account Manager is an automation tool designed to streamline the management of Mega account files. The project reads Mega account credentials from an Excel file, automatically logs in to each account, and retrieves detailed file information. It performs a variety of management actions on the files, making the entire process seamless and efficient.

## Features

- **Excel Integration:** Reads Mega account credentials from an Excel file.
- **Auto Login:** Automatically logs in to Mega accounts using the credentials.
- **File Information Retrieval:** Fetches detailed information about all files in the account.
- **File Management Actions:** Executes predefined management actions on the retrieved files.

## Technologies Used

- .NET Core and BlazorWasm
- Pandas (for Excel file handling)
- MegaAPIClient nuget package (for interacting with Mega accounts)

## Installation

1. Clone the repository:
    ```shell
    git clone https://github.com/Albie-Dev/ManageMegaAccount.git
    cd megacloud-manager
    ```

2. Install the required dependencies:
    ```shell
    dotnet restore
    ```

3. Update the `accounts.xlsx` file with your Mega account credentials.

## Usage

1. Run the script:
    ```shell
    dotnet run
    ```

2. The script will read the account information from the `accounts.xlsx` file, log in to each account, and perform the defined file management actions.

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
