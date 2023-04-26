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

resource "azurerm_resource_group" "iaccomunidadecloud" {
  name = "iaccomunidadecloud"
  location = "Central US"
}

resource "azurerm_service_plan" "iaccomunidadecloudsp" {
  name = "iaccomunidadecloudsp"
  resource_group_name = azurerm_resource_group.iaccomunidadecloud.name
  location = "Central US"
  os_type = "Linux"
  sku_name = "F1"
}

resource "azurerm_linux_web_app" "iaccomunidadecloud" {
  resource_group_name = azurerm_resource_group.iaccomunidadecloud.name
  name = "iaccomunidadecloud"
  location = azurerm_service_plan.iaccomunidadecloudsp.location
  service_plan_id = azurerm_service_plan.iaccomunidadecloudsp.id

  site_config {
    always_on = false
    ftps_state = "AllAllowed"
    container_registry_use_managed_identity = false
    application_stack {
      docker_image = "acrcomunidadecloud.azurecr.io/mycontainer"
      docker_image_tag = "latest"
    }
  }
}