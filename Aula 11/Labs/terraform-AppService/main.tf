terraform{
    required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.55.0"
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

resource "azurerm_resource_group" "iaccomunidadecloud" {
  name = "postcard_app_service"
  location = "East US"
}

resource "azurerm_service_plan" "iaccomunidadecloudsp" {
  name = "postcard_app_service_SP"
  resource_group_name = azurerm_resource_group.iaccomunidadecloud.name
  location = "East US"
  os_type = "Linux"
  sku_name = "B1"
}

resource "azurerm_linux_web_app" "iaccomunidadecloud" {
  resource_group_name = azurerm_resource_group.iaccomunidadecloud.name
  name = "postcard-augroberto"
  location = azurerm_service_plan.iaccomunidadecloudsp.location
  service_plan_id = azurerm_service_plan.iaccomunidadecloudsp.id

  site_config {
    always_on = false
    ftps_state = "AllAllowed"
    container_registry_use_managed_identity = false
    application_stack {
      docker_image = "postcard.azurecr.io/postcard"
      docker_image_tag = "latest"
    }
  }
}