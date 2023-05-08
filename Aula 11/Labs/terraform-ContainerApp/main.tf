terraform{
    required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.55.0"
    }
  }
  #backend "azurerm" {
  #  resource_group_name  = "tamopstfstates"
  #  storage_account_name = "cctamopstf"
  #  container_name       = "tfstatedevops"
  #  key                  = "terraform.tfstate"
  #}
}

provider "azurerm" {
    features {}

    subscription_id = "00acdf14-1bef-48ab-afa0-f9a3fdc222fe"
    tenant_id = "62270788-df1d-4fba-9177-debe2d4574d4"
}

resource "azurerm_resource_group" "postcard" {
  name = "postcard-rg"
  location = "East US"
}

resource "azurerm_log_analytics_workspace" "loganalytics" {
  name                = "postcard-logs"
  location            = azurerm_resource_group.postcard.location
  resource_group_name = azurerm_resource_group.postcard.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "containerappenv" {
  name                       = "postcard-containerappenv"
  location                   = azurerm_resource_group.postcard.location
  resource_group_name        = azurerm_resource_group.postcard.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.loganalytics.id
}

resource "azurerm_container_app" "containerapp" {
  name                         = "postcard-app"
  container_app_environment_id = azurerm_container_app_environment.containerappenv.id
  resource_group_name          = azurerm_resource_group.postcard.name
  revision_mode                = "Single"

  template {
    container {
      name   = "postcardapp"
      image  = "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest"
      #image  = "postcard.azurecr.io/postcard:15"
      cpu    = 0.25
      memory = "0.5Gi"
    }
  }

  ingress {
      external_enabled = true
      target_port = 80
      traffic_weight {
        percentage = 100
      }
  }
}