terraform{
    required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.0.0"
    }
  }
  backend "azurerm" {
    resource_group_name  = "tamopstfstates"
    storage_account_name = "cctamopstf"
    container_name       = "tfstatedevops"
    key                  = "terraform.tfstate"
  }
}

provider "azurerm" {
    features {}

    subscription_id = "d7db7887-0e34-451e-98c6-a22c3c394233"
    tenant_id = "f5237a32-dd33-498a-9dff-dfb70f5e8ac7"
}

resource "azurerm_resource_group" "postcard" {
  name = "postcard"
  location = "Central US"
}

resource "azurerm_container_app_environment" "containerappenv" {
  name                       = "${var.aca_name}containerappenv"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.loganalytics.id
}

resource "azurerm_container_app" "containerapp" {
  name                         = "${var.aca_name}app"
  container_app_environment_id = azurerm_container_app_environment.containerappenv.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Multiple"
}