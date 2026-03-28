const i18nKeyContainer = {
  common: {
    languages: {
      english: 'common.languages.english',
      french: 'common.languages.french',
      arabic: 'common.languages.arabic',
    },
  },
  layout: {
    sidebar: {
      brand: 'layout.sidebar.brand',
      menu: {
        dashboard: 'layout.sidebar.menu.dashboard',
        products: 'layout.sidebar.menu.products',
        inventory: 'layout.sidebar.menu.inventory',
        sales: 'layout.sidebar.menu.sales',
        customers: 'layout.sidebar.menu.customers',
        settings: 'layout.sidebar.menu.settings',
      },
    },
    topNav: {
      languageSwitcher: 'layout.topNav.languageSwitcher',
      searchPlaceholder: 'layout.topNav.searchPlaceholder',
    },
  },
  dashboard: {
    page: {
      title: 'dashboard.page.title',
      description: 'dashboard.page.description',
      generateReport: 'dashboard.page.generateReport',
    },
    cards: {
      totalProducts: {
        title: 'dashboard.cards.totalProducts.title',
        description: 'dashboard.cards.totalProducts.description',
      },
      totalCustomers: {
        title: 'dashboard.cards.totalCustomers.title',
        description: 'dashboard.cards.totalCustomers.description',
      },
      lowStockItems: {
        title: 'dashboard.cards.lowStockItems.title',
        description: 'dashboard.cards.lowStockItems.description',
      },
      totalSalesOrders: {
        title: 'dashboard.cards.totalSalesOrders.title',
        description: 'dashboard.cards.totalSalesOrders.description',
      },
      totalRevenues: {
        title: 'dashboard.cards.totalRevenues.title',
        description: 'dashboard.cards.totalRevenues.description',
      },
      pendingOrders: {
        title: 'dashboard.cards.pendingOrders.title',
        description: 'dashboard.cards.pendingOrders.description',
      },
      fulfilledOrders: {
        title: 'dashboard.cards.fulfilledOrders.title',
        description: 'dashboard.cards.fulfilledOrders.description',
      },
      activeSuppliers: {
        title: 'dashboard.cards.activeSuppliers.title',
        description: 'dashboard.cards.activeSuppliers.description',
      },
    },
    sections: {
      topSellingProducts: 'dashboard.sections.topSellingProducts',
      quickActions: 'dashboard.sections.quickActions',
      inventoryAlerts: 'dashboard.sections.inventoryAlerts',
    },
    todayPerformance: {
      title: 'dashboard.todayPerformance.title',
      todaysSales: 'dashboard.todayPerformance.todaysSales',
      newOrders: 'dashboard.todayPerformance.newOrders',
      newCustomers: 'dashboard.todayPerformance.newCustomers',
      productsSold: 'dashboard.todayPerformance.productsSold',
    },
    quickActions: {
      addProduct: 'dashboard.quickActions.addProduct',
      addCustomer: 'dashboard.quickActions.addCustomer',
      newSaleOrder: 'dashboard.quickActions.newSaleOrder',
      addLocation: 'dashboard.quickActions.addLocation',
    },
    topProducts: {
      loading: 'dashboard.topProducts.loading',
      unitsSold: 'dashboard.topProducts.unitsSold',
      revenue: 'dashboard.topProducts.revenue',
    },
  },
  products: {
    shared: {
      loading: 'products.shared.loading',
      close: 'products.shared.close',
      notSpecified: 'products.shared.notSpecified',
      noDescriptionProvided: 'products.shared.noDescriptionProvided',
      hyphen: 'products.shared.hyphen',
      required: 'products.shared.required',
      status: {
        active: 'products.shared.status.active',
        inactive: 'products.shared.status.inactive',
        draft: 'products.shared.status.draft',
        deleted: 'products.shared.status.deleted',
      },
    },
    page: {
      title: 'products.page.title',
      description: 'products.page.description',
      addProduct: 'products.page.addProduct',
      tabs: {
        products: 'products.page.tabs.products',
        stockMovements: 'products.page.tabs.stockMovements',
        unitOfMeasure: 'products.page.tabs.unitOfMeasure',
        productCategories: 'products.page.tabs.productCategories',
        productImages: 'products.page.tabs.productImages',
      },
      sections: {
        productCatalog: 'products.page.sections.productCatalog',
        stockMovementHistory: 'products.page.sections.stockMovementHistory',
        unitOfMeasure: 'products.page.sections.unitOfMeasure',
        productCategories: 'products.page.sections.productCategories',
        productImages: 'products.page.sections.productImages',
      },
      placeholders: {
        productImages: 'products.page.placeholders.productImages',
      },
    },
    cards: {
      totalProducts: {
        title: 'products.cards.totalProducts.title',
        description: 'products.cards.totalProducts.description',
      },
      inventoryValue: {
        title: 'products.cards.inventoryValue.title',
        description: 'products.cards.inventoryValue.description',
      },
      lowStockItems: {
        title: 'products.cards.lowStockItems.title',
        description: 'products.cards.lowStockItems.description',
      },
      profitPotential: {
        title: 'products.cards.profitPotential.title',
        description: 'products.cards.profitPotential.description',
      },
    },
    addProductForm: {
      title: {
        add: 'products.addProductForm.title.add',
        edit: 'products.addProductForm.title.edit',
      },
      tabs: {
        basicInfo: 'products.addProductForm.tabs.basicInfo',
        pricing: 'products.addProductForm.tabs.pricing',
        inventory: 'products.addProductForm.tabs.inventory',
        details: 'products.addProductForm.tabs.details',
      },
      sections: {
        productInformation: 'products.addProductForm.sections.productInformation',
        pricingInformation: 'products.addProductForm.sections.pricingInformation',
        inventoryManagement: 'products.addProductForm.sections.inventoryManagement',
        additionalDetails: 'products.addProductForm.sections.additionalDetails',
        productSummary: 'products.addProductForm.sections.productSummary',
      },
      fields: {
        productName: 'products.addProductForm.fields.productName',
        sku: 'products.addProductForm.fields.sku',
        category: 'products.addProductForm.fields.category',
        description: 'products.addProductForm.fields.description',
        status: 'products.addProductForm.fields.status',
        costPrice: 'products.addProductForm.fields.costPrice',
        sellingPrice: 'products.addProductForm.fields.sellingPrice',
        currentStock: 'products.addProductForm.fields.currentStock',
        minimumStock: 'products.addProductForm.fields.minimumStock',
        maximumStock: 'products.addProductForm.fields.maximumStock',
        unitOfMeasurement: 'products.addProductForm.fields.unitOfMeasurement',
        storageLocation: 'products.addProductForm.fields.storageLocation',
      },
      placeholders: {
        productName: 'products.addProductForm.placeholders.productName',
        sku: 'products.addProductForm.placeholders.sku',
        description: 'products.addProductForm.placeholders.description',
        zero: 'products.addProductForm.placeholders.zero',
      },
      metrics: {
        profitMargin: 'products.addProductForm.metrics.profitMargin',
        profitPerUnit: 'products.addProductForm.metrics.profitPerUnit',
        markup: 'products.addProductForm.metrics.markup',
      },
      summary: {
        productName: 'products.addProductForm.summary.productName',
        sku: 'products.addProductForm.summary.sku',
        category: 'products.addProductForm.summary.category',
        sellingPrice: 'products.addProductForm.summary.sellingPrice',
        initialStock: 'products.addProductForm.summary.initialStock',
        status: 'products.addProductForm.summary.status',
      },
      actions: {
        cancel: 'products.addProductForm.actions.cancel',
        addProduct: 'products.addProductForm.actions.addProduct',
        saveChanges: 'products.addProductForm.actions.saveChanges',
      },
      toasts: {
        createSuccessTitle: 'products.addProductForm.toasts.createSuccessTitle',
        createSuccessMessage:
          'products.addProductForm.toasts.createSuccessMessage',
        createErrorTitle: 'products.addProductForm.toasts.createErrorTitle',
        createErrorMessage: 'products.addProductForm.toasts.createErrorMessage',
        updateSuccessTitle: 'products.addProductForm.toasts.updateSuccessTitle',
        updateSuccessMessage:
          'products.addProductForm.toasts.updateSuccessMessage',
        updateErrorTitle: 'products.addProductForm.toasts.updateErrorTitle',
        updateErrorMessage: 'products.addProductForm.toasts.updateErrorMessage',
      },
    },
    productTable: {
      columns: {
        sku: 'products.productTable.columns.sku',
        product: 'products.productTable.columns.product',
        stock: 'products.productTable.columns.stock',
        price: 'products.productTable.columns.price',
        cost: 'products.productTable.columns.cost',
        isActive: 'products.productTable.columns.isActive',
        createdAt: 'products.productTable.columns.createdAt',
      },
      dialogs: {
        deleteTitle: 'products.productTable.dialogs.deleteTitle',
        deleteMessage: 'products.productTable.dialogs.deleteMessage',
      },
      toasts: {
        deleteCancelledTitle: 'products.productTable.toasts.deleteCancelledTitle',
        deleteCancelledMessage:
          'products.productTable.toasts.deleteCancelledMessage',
        deleteSuccessTitle: 'products.productTable.toasts.deleteSuccessTitle',
        deleteSuccessMessage: 'products.productTable.toasts.deleteSuccessMessage',
      },
    },
    productView: {
      title: 'products.productView.title',
      tabs: {
        basicInfo: 'products.productView.tabs.basicInfo',
        pricing: 'products.productView.tabs.pricing',
        details: 'products.productView.tabs.details',
      },
      sections: {
        productInformation: 'products.productView.sections.productInformation',
        pricingInformation: 'products.productView.sections.pricingInformation',
        creationDetails: 'products.productView.sections.creationDetails',
        lastUpdateDetails: 'products.productView.sections.lastUpdateDetails',
        systemInformation: 'products.productView.sections.systemInformation',
        deletionInformation: 'products.productView.sections.deletionInformation',
      },
      fields: {
        productName: 'products.productView.fields.productName',
        sku: 'products.productView.fields.sku',
        category: 'products.productView.fields.category',
        unitOfMeasure: 'products.productView.fields.unitOfMeasure',
        description: 'products.productView.fields.description',
        status: 'products.productView.fields.status',
        costPrice: 'products.productView.fields.costPrice',
        sellingPrice: 'products.productView.fields.sellingPrice',
        profitMargin: 'products.productView.fields.profitMargin',
        profitPerUnit: 'products.productView.fields.profitPerUnit',
        markup: 'products.productView.fields.markup',
        createdAt: 'products.productView.fields.createdAt',
        createdBy: 'products.productView.fields.createdBy',
        createdByUserId: 'products.productView.fields.createdByUserId',
        updatedAt: 'products.productView.fields.updatedAt',
        updatedBy: 'products.productView.fields.updatedBy',
        updatedByUserId: 'products.productView.fields.updatedByUserId',
        productId: 'products.productView.fields.productId',
        categoryId: 'products.productView.fields.categoryId',
        unitOfMeasureId: 'products.productView.fields.unitOfMeasureId',
        deletedAt: 'products.productView.fields.deletedAt',
        deletedBy: 'products.productView.fields.deletedBy',
      },
      placeholders: {
        noDescriptionAvailable:
          'products.productView.placeholders.noDescriptionAvailable',
      },
    },
    stockMovements: {
      columns: {
        date: 'products.stockMovements.columns.date',
        product: 'products.stockMovements.columns.product',
        type: 'products.stockMovements.columns.type',
        quantity: 'products.stockMovements.columns.quantity',
        reason: 'products.stockMovements.columns.reason',
        processedBy: 'products.stockMovements.columns.processedBy',
      },
      badges: {
        in: 'products.stockMovements.badges.in',
        out: 'products.stockMovements.badges.out',
        transfer: 'products.stockMovements.badges.transfer',
        unknown: 'products.stockMovements.badges.unknown',
      },
    },
    categories: {
      buttonAdd: 'products.categories.buttonAdd',
      form: {
        title: {
          add: 'products.categories.form.title.add',
          edit: 'products.categories.form.title.edit',
        },
        sectionInfo: 'products.categories.form.sectionInfo',
        fields: {
          categoryName: 'products.categories.form.fields.categoryName',
          description: 'products.categories.form.fields.description',
          categoryType: 'products.categories.form.fields.categoryType',
          mainCategory: 'products.categories.form.fields.mainCategory',
          mainCategoryDescription:
            'products.categories.form.fields.mainCategoryDescription',
          subcategory: 'products.categories.form.fields.subcategory',
          subcategoryDescription:
            'products.categories.form.fields.subcategoryDescription',
          parentCategory: 'products.categories.form.fields.parentCategory',
        },
        placeholders: {
          categoryName: 'products.categories.form.placeholders.categoryName',
          description: 'products.categories.form.placeholders.description',
          parentCategory: 'products.categories.form.placeholders.parentCategory',
        },
        validation: {
          categoryNameRequired:
            'products.categories.form.validation.categoryNameRequired',
          parentCategoryRequired:
            'products.categories.form.validation.parentCategoryRequired',
        },
        actions: {
          cancel: 'products.categories.form.actions.cancel',
          create: 'products.categories.form.actions.create',
          saveChanges: 'products.categories.form.actions.saveChanges',
          creating: 'products.categories.form.actions.creating',
          saving: 'products.categories.form.actions.saving',
        },
        toasts: {
          createSuccessTitle: 'products.categories.form.toasts.createSuccessTitle',
          createSuccessMessage:
            'products.categories.form.toasts.createSuccessMessage',
          createErrorTitle: 'products.categories.form.toasts.createErrorTitle',
          createErrorMessage: 'products.categories.form.toasts.createErrorMessage',
          updateSuccessTitle: 'products.categories.form.toasts.updateSuccessTitle',
          updateSuccessMessage:
            'products.categories.form.toasts.updateSuccessMessage',
          updateErrorTitle: 'products.categories.form.toasts.updateErrorTitle',
          updateErrorMessage: 'products.categories.form.toasts.updateErrorMessage',
          loadErrorTitle: 'products.categories.form.toasts.loadErrorTitle',
          loadErrorMessage: 'products.categories.form.toasts.loadErrorMessage',
        },
      },
      table: {
        dialogs: {
          deleteTitle: 'products.categories.table.dialogs.deleteTitle',
          deleteMessage: 'products.categories.table.dialogs.deleteMessage',
        },
        toasts: {
          deleteSuccess: 'products.categories.table.toasts.deleteSuccess',
          deleteError: 'products.categories.table.toasts.deleteError',
        },
        columns: {
          id: 'products.categories.table.columns.id',
          name: 'products.categories.table.columns.name',
          parentCategory: 'products.categories.table.columns.parentCategory',
          mainCategory: 'products.categories.table.columns.mainCategory',
          createdAt: 'products.categories.table.columns.createdAt',
          createdBy: 'products.categories.table.columns.createdBy',
        },
      },
      view: {
        title: 'products.categories.view.title',
        subtitleId: 'products.categories.view.subtitleId',
        loading: 'products.categories.view.loading',
        sections: {
          categoryInformation: 'products.categories.view.sections.categoryInformation',
          subcategories: 'products.categories.view.sections.subcategories',
          auditInformation: 'products.categories.view.sections.auditInformation',
          created: 'products.categories.view.sections.created',
          updated: 'products.categories.view.sections.updated',
        },
        badges: {
          mainCategory: 'products.categories.view.badges.mainCategory',
          subCategory: 'products.categories.view.badges.subCategory',
        },
        fields: {
          categoryName: 'products.categories.view.fields.categoryName',
          parentCategory: 'products.categories.view.fields.parentCategory',
          description: 'products.categories.view.fields.description',
          dateTime: 'products.categories.view.fields.dateTime',
          user: 'products.categories.view.fields.user',
        },
        placeholders: {
          noDescription: 'products.categories.view.placeholders.noDescription',
          noSubcategories: 'products.categories.view.placeholders.noSubcategories',
          adminUser: 'products.categories.view.placeholders.adminUser',
          neverUpdated: 'products.categories.view.placeholders.neverUpdated',
        },
      },
    },
    units: {
      buttonAdd: 'products.units.buttonAdd',
      form: {
        title: {
          add: 'products.units.form.title.add',
          edit: 'products.units.form.title.edit',
        },
        sectionInfo: 'products.units.form.sectionInfo',
        fields: {
          unitName: 'products.units.form.fields.unitName',
          description: 'products.units.form.fields.description',
        },
        placeholders: {
          unitName: 'products.units.form.placeholders.unitName',
          description: 'products.units.form.placeholders.description',
        },
        validation: {
          unitNameRequired: 'products.units.form.validation.unitNameRequired',
        },
        actions: {
          cancel: 'products.units.form.actions.cancel',
          create: 'products.units.form.actions.create',
          saveChanges: 'products.units.form.actions.saveChanges',
          creating: 'products.units.form.actions.creating',
          saving: 'products.units.form.actions.saving',
        },
        toasts: {
          createSuccessTitle: 'products.units.form.toasts.createSuccessTitle',
          createSuccessMessage: 'products.units.form.toasts.createSuccessMessage',
          createErrorTitle: 'products.units.form.toasts.createErrorTitle',
          createErrorMessage: 'products.units.form.toasts.createErrorMessage',
          updateSuccessTitle: 'products.units.form.toasts.updateSuccessTitle',
          updateSuccessMessage: 'products.units.form.toasts.updateSuccessMessage',
          updateErrorTitle: 'products.units.form.toasts.updateErrorTitle',
          updateErrorMessage: 'products.units.form.toasts.updateErrorMessage',
          loadErrorTitle: 'products.units.form.toasts.loadErrorTitle',
          loadErrorMessage: 'products.units.form.toasts.loadErrorMessage',
        },
      },
      table: {
        dialogs: {
          deleteTitle: 'products.units.table.dialogs.deleteTitle',
          deleteMessage: 'products.units.table.dialogs.deleteMessage',
        },
        toasts: {
          deleteSuccessTitle: 'products.units.table.toasts.deleteSuccessTitle',
          deleteSuccessMessage: 'products.units.table.toasts.deleteSuccessMessage',
          deleteError: 'products.units.table.toasts.deleteError',
          loadError: 'products.units.table.toasts.loadError',
        },
        columns: {
          id: 'products.units.table.columns.id',
          name: 'products.units.table.columns.name',
          createdAt: 'products.units.table.columns.createdAt',
          createdBy: 'products.units.table.columns.createdBy',
        },
      },
      view: {
        title: 'products.units.view.title',
        subtitleId: 'products.units.view.subtitleId',
        sections: {
          unitInformation: 'products.units.view.sections.unitInformation',
          auditInformation: 'products.units.view.sections.auditInformation',
          created: 'products.units.view.sections.created',
          updated: 'products.units.view.sections.updated',
        },
        fields: {
          unitName: 'products.units.view.fields.unitName',
          description: 'products.units.view.fields.description',
          dateTime: 'products.units.view.fields.dateTime',
          user: 'products.units.view.fields.user',
        },
        placeholders: {
          noDescription: 'products.units.view.placeholders.noDescription',
          adminUser: 'products.units.view.placeholders.adminUser',
          neverUpdated: 'products.units.view.placeholders.neverUpdated',
        },
      },
    },
    demo: {
      addProduct: {
        title: 'products.demo.addProduct.title',
        description: 'products.demo.addProduct.description',
        addButton: 'products.demo.addProduct.addButton',
        listTitle: 'products.demo.addProduct.listTitle',
        emptyState: 'products.demo.addProduct.emptyState',
        alerts: {
          addSuccess: 'products.demo.addProduct.alerts.addSuccess',
          addError: 'products.demo.addProduct.alerts.addError',
        },
        labels: {
          sku: 'products.demo.addProduct.labels.sku',
          category: 'products.demo.addProduct.labels.category',
          brand: 'products.demo.addProduct.labels.brand',
          stock: 'products.demo.addProduct.labels.stock',
          status: 'products.demo.addProduct.labels.status',
        },
      },
      deleteExample: {
        title: 'products.demo.deleteExample.title',
        message: 'products.demo.deleteExample.message',
        toastSuccessTitle: 'products.demo.deleteExample.toastSuccessTitle',
        toastSuccessMessage: 'products.demo.deleteExample.toastSuccessMessage',
        toastErrorTitle: 'products.demo.deleteExample.toastErrorTitle',
        toastErrorMessage: 'products.demo.deleteExample.toastErrorMessage',
      },
    },
  },
  customers: {
    shared: {
      loading: 'customers.shared.loading',
      notAvailable: 'customers.shared.notAvailable',
      required: 'customers.shared.required',
      currencySymbol: 'customers.shared.currencySymbol',
      status: {
        active: 'customers.shared.status.active',
        inactive: 'customers.shared.status.inactive',
      },
      creditStatus: {
        active: 'customers.shared.creditStatus.active',
        onHold: 'customers.shared.creditStatus.onHold',
        suspended: 'customers.shared.creditStatus.suspended',
      },
      defaults: {
        paymentTerms: 'customers.shared.defaults.paymentTerms',
        customerCategory: 'customers.shared.defaults.customerCategory',
        createdBy: 'customers.shared.defaults.createdBy',
      },
    },
    page: {
      title: 'customers.page.title',
      description: 'customers.page.description',
      tabs: {
        customers: 'customers.page.tabs.customers',
        customerCategories: 'customers.page.tabs.customerCategories',
        customerContacts: 'customers.page.tabs.customerContacts',
      },
      sections: {
        customerCatalog: 'customers.page.sections.customerCatalog',
        customerCategories: 'customers.page.sections.customerCategories',
        customerContacts: 'customers.page.sections.customerContacts',
      },
      placeholders: {
        customerCategories: 'customers.page.placeholders.customerCategories',
        customerContacts: 'customers.page.placeholders.customerContacts',
      },
    },
    cards: {
      totalCustomers: {
        title: 'customers.cards.totalCustomers.title',
        description: 'customers.cards.totalCustomers.description',
      },
      totalRevenue: {
        title: 'customers.cards.totalRevenue.title',
        description: 'customers.cards.totalRevenue.description',
      },
      newCustomers: {
        title: 'customers.cards.newCustomers.title',
        description: 'customers.cards.newCustomers.description',
      },
      outOfStockItems: {
        title: 'customers.cards.outOfStockItems.title',
        description: 'customers.cards.outOfStockItems.description',
        value: 'customers.cards.outOfStockItems.value',
      },
    },
    addButton: {
      label: 'customers.addButton.label',
    },
    form: {
      title: {
        add: 'customers.form.title.add',
        edit: 'customers.form.title.edit',
      },
      subtitle: {
        add: 'customers.form.subtitle.add',
        edit: 'customers.form.subtitle.edit',
      },
      tabs: {
        basicInfo: 'customers.form.tabs.basicInfo',
        business: 'customers.form.tabs.business',
        summary: 'customers.form.tabs.summary',
      },
      sections: {
        personalDetails: 'customers.form.sections.personalDetails',
        address: 'customers.form.sections.address',
        businessDetails: 'customers.form.sections.businessDetails',
      },
      fields: {
        fullName: 'customers.form.fields.fullName',
        email: 'customers.form.fields.email',
        phone: 'customers.form.fields.phone',
        customerType: 'customers.form.fields.customerType',
        street: 'customers.form.fields.street',
        city: 'customers.form.fields.city',
        state: 'customers.form.fields.state',
        zipCode: 'customers.form.fields.zipCode',
        creditLimit: 'customers.form.fields.creditLimit',
        creditStatus: 'customers.form.fields.creditStatus',
        paymentTerms: 'customers.form.fields.paymentTerms',
      },
      placeholders: {
        fullName: 'customers.form.placeholders.fullName',
        email: 'customers.form.placeholders.email',
        phone: 'customers.form.placeholders.phone',
        customerType: 'customers.form.placeholders.customerType',
        street: 'customers.form.placeholders.street',
        city: 'customers.form.placeholders.city',
        state: 'customers.form.placeholders.state',
        zipCode: 'customers.form.placeholders.zipCode',
        creditLimit: 'customers.form.placeholders.creditLimit',
        paymentTerms: 'customers.form.placeholders.paymentTerms',
      },
      validation: {
        fullNameRequired: 'customers.form.validation.fullNameRequired',
        emailRequired: 'customers.form.validation.emailRequired',
        invalidEmail: 'customers.form.validation.invalidEmail',
        phoneRequired: 'customers.form.validation.phoneRequired',
        customerTypeRequired: 'customers.form.validation.customerTypeRequired',
        streetRequired: 'customers.form.validation.streetRequired',
        cityRequired: 'customers.form.validation.cityRequired',
        stateRequired: 'customers.form.validation.stateRequired',
        zipCodeRequired: 'customers.form.validation.zipCodeRequired',
        creditLimitPositive: 'customers.form.validation.creditLimitPositive',
      },
      actions: {
        previous: 'customers.form.actions.previous',
        cancel: 'customers.form.actions.cancel',
        close: 'customers.form.actions.close',
        saveCustomer: 'customers.form.actions.saveCustomer',
        saveChanges: 'customers.form.actions.saveChanges',
        next: 'customers.form.actions.next',
        saving: 'customers.form.actions.saving',
      },
      toasts: {
        validationErrorTitle: 'customers.form.toasts.validationErrorTitle',
        validationErrorMessage: 'customers.form.toasts.validationErrorMessage',
        createSuccessTitle: 'customers.form.toasts.createSuccessTitle',
        createSuccessMessage: 'customers.form.toasts.createSuccessMessage',
        createFailedTitle: 'customers.form.toasts.createFailedTitle',
        createFailedMessage: 'customers.form.toasts.createFailedMessage',
        updateSuccessTitle: 'customers.form.toasts.updateSuccessTitle',
        updateSuccessMessage: 'customers.form.toasts.updateSuccessMessage',
        updateFailedTitle: 'customers.form.toasts.updateFailedTitle',
        updateFailedMessage: 'customers.form.toasts.updateFailedMessage',
        loadFailedTitle: 'customers.form.toasts.loadFailedTitle',
        loadFailedMessage: 'customers.form.toasts.loadFailedMessage',
      },
    },
    table: {
      columns: {
        customer: 'customers.table.columns.customer',
        phone: 'customers.table.columns.phone',
        category: 'customers.table.columns.category',
        totalOrders: 'customers.table.columns.totalOrders',
        totalSpent: 'customers.table.columns.totalSpent',
        isActive: 'customers.table.columns.isActive',
        createdAt: 'customers.table.columns.createdAt',
      },
      dialogs: {
        deleteTitle: 'customers.table.dialogs.deleteTitle',
        deleteMessage: 'customers.table.dialogs.deleteMessage',
      },
      toasts: {
        deleteFailedTitle: 'customers.table.toasts.deleteFailedTitle',
        deleteFailedMessage: 'customers.table.toasts.deleteFailedMessage',
        deleteSuccessTitle: 'customers.table.toasts.deleteSuccessTitle',
        deleteSuccessMessage: 'customers.table.toasts.deleteSuccessMessage',
      },
    },
    view: {
      loading: 'customers.view.loading',
      noCustomerData: 'customers.view.noCustomerData',
      sections: {
        generalInformation: 'customers.view.sections.generalInformation',
        address: 'customers.view.sections.address',
        businessInformation: 'customers.view.sections.businessInformation',
        systemInformation: 'customers.view.sections.systemInformation',
      },
      fields: {
        id: 'customers.view.fields.id',
        name: 'customers.view.fields.name',
        customerCategory: 'customers.view.fields.customerCategory',
        email: 'customers.view.fields.email',
        phone: 'customers.view.fields.phone',
        status: 'customers.view.fields.status',
        street: 'customers.view.fields.street',
        city: 'customers.view.fields.city',
        state: 'customers.view.fields.state',
        zipCode: 'customers.view.fields.zipCode',
        creditLimit: 'customers.view.fields.creditLimit',
        paymentTerms: 'customers.view.fields.paymentTerms',
        creditStatus: 'customers.view.fields.creditStatus',
        createdAt: 'customers.view.fields.createdAt',
        createdBy: 'customers.view.fields.createdBy',
      },
      labels: {
        customerCode: 'customers.view.labels.customerCode',
      },
    },
  },
  inventory: {
    shared: {
      loading: 'inventory.shared.loading',
      close: 'inventory.shared.close',
      cancel: 'inventory.shared.cancel',
      search: 'inventory.shared.search',
      saveChanges: 'inventory.shared.saveChanges',
      creating: 'inventory.shared.creating',
      saving: 'inventory.shared.saving',
      processing: 'inventory.shared.processing',
      notSpecified: 'inventory.shared.notSpecified',
      notAvailable: 'inventory.shared.notAvailable',
      noAddressProvided: 'inventory.shared.noAddressProvided',
      noDescriptionProvided: 'inventory.shared.noDescriptionProvided',
      required: 'inventory.shared.required',
      yes: 'inventory.shared.yes',
      no: 'inventory.shared.no',
      systemUser: 'inventory.shared.systemUser',
      adminUser: 'inventory.shared.adminUser',
      unknownUser: 'inventory.shared.unknownUser',
      status: {
        active: 'inventory.shared.status.active',
        inactive: 'inventory.shared.status.inactive',
        inStock: 'inventory.shared.status.inStock',
        lowStock: 'inventory.shared.status.lowStock',
        outOfStock: 'inventory.shared.status.outOfStock',
        pending: 'inventory.shared.status.pending',
        approved: 'inventory.shared.status.approved',
        inTransit: 'inventory.shared.status.inTransit',
        completed: 'inventory.shared.status.completed',
        cancelled: 'inventory.shared.status.cancelled',
        rejected: 'inventory.shared.status.rejected',
        failed: 'inventory.shared.status.failed',
        deleted: 'inventory.shared.status.deleted',
      },
    },
    page: {
      title: 'inventory.page.title',
      description: 'inventory.page.description',
      addInventory: 'inventory.page.addInventory',
      tabs: {
        inventory: 'inventory.page.tabs.inventory',
        stockMovements: 'inventory.page.tabs.stockMovements',
        stockTransfers: 'inventory.page.tabs.stockTransfers',
        stockAlerts: 'inventory.page.tabs.stockAlerts',
        locations: 'inventory.page.tabs.locations',
      },
      sections: {
        inventoryCatalog: 'inventory.page.sections.inventoryCatalog',
        stockMovements: 'inventory.page.sections.stockMovements',
        stockTransfers: 'inventory.page.sections.stockTransfers',
        stockAlerts: 'inventory.page.sections.stockAlerts',
        locations: 'inventory.page.sections.locations',
      },
      placeholders: {
        stockAlerts: 'inventory.page.placeholders.stockAlerts',
      },
    },
    cards: {
      totalInventoryItems: {
        title: 'inventory.cards.totalInventoryItems.title',
        description: 'inventory.cards.totalInventoryItems.description',
      },
      totalPotentialProfit: {
        title: 'inventory.cards.totalPotentialProfit.title',
        description: 'inventory.cards.totalPotentialProfit.description',
      },
      lowStockItems: {
        title: 'inventory.cards.lowStockItems.title',
        description: 'inventory.cards.lowStockItems.description',
      },
      outOfStockItems: {
        title: 'inventory.cards.outOfStockItems.title',
        description: 'inventory.cards.outOfStockItems.description',
      },
    },
    addInventoryButton: {
      ariaLabel: 'inventory.addInventoryButton.ariaLabel',
      label: 'inventory.addInventoryButton.label',
    },
    inventoryForm: {
      title: {
        add: 'inventory.inventoryForm.title.add',
        edit: 'inventory.inventoryForm.title.edit',
      },
      tabs: {
        product: 'inventory.inventoryForm.tabs.product',
        location: 'inventory.inventoryForm.tabs.location',
        stockLevels: 'inventory.inventoryForm.tabs.stockLevels',
      },
      sections: {
        productSearch: 'inventory.inventoryForm.sections.productSearch',
        locationSelection: 'inventory.inventoryForm.sections.locationSelection',
        stockLevels: 'inventory.inventoryForm.sections.stockLevels',
        productInformation: 'inventory.inventoryForm.sections.productInformation',
        locationInformation:
          'inventory.inventoryForm.sections.locationInformation',
        summary: 'inventory.inventoryForm.sections.summary',
        currentAvailableStock:
          'inventory.inventoryForm.sections.currentAvailableStock',
      },
      fields: {
        searchProduct: 'inventory.inventoryForm.fields.searchProduct',
        productName: 'inventory.inventoryForm.fields.productName',
        productSku: 'inventory.inventoryForm.fields.productSku',
        category: 'inventory.inventoryForm.fields.category',
        unitOfMeasure: 'inventory.inventoryForm.fields.unitOfMeasure',
        productId: 'inventory.inventoryForm.fields.productId',
        selectLocation: 'inventory.inventoryForm.fields.selectLocation',
        locationName: 'inventory.inventoryForm.fields.locationName',
        locationId: 'inventory.inventoryForm.fields.locationId',
        address: 'inventory.inventoryForm.fields.address',
        type: 'inventory.inventoryForm.fields.type',
        quantityOnHand: 'inventory.inventoryForm.fields.quantityOnHand',
        reorderLevel: 'inventory.inventoryForm.fields.reorderLevel',
        maxLevel: 'inventory.inventoryForm.fields.maxLevel',
      },
      placeholders: {
        searchProduct: 'inventory.inventoryForm.placeholders.searchProduct',
        selectLocation: 'inventory.inventoryForm.placeholders.selectLocation',
        searchPrompt: 'inventory.inventoryForm.placeholders.searchPrompt',
        selectLocationPrompt:
          'inventory.inventoryForm.placeholders.selectLocationPrompt',
        enterQuantity: 'inventory.inventoryForm.placeholders.enterQuantity',
        enterReorderLevel:
          'inventory.inventoryForm.placeholders.enterReorderLevel',
        enterMaxLevel: 'inventory.inventoryForm.placeholders.enterMaxLevel',
      },
      hints: {
        productReadonly: 'inventory.inventoryForm.hints.productReadonly',
        locationReadonly: 'inventory.inventoryForm.hints.locationReadonly',
        quantityOnHand: 'inventory.inventoryForm.hints.quantityOnHand',
        reorderLevel: 'inventory.inventoryForm.hints.reorderLevel',
        maxLevel: 'inventory.inventoryForm.hints.maxLevel',
        completeProductAndLocation:
          'inventory.inventoryForm.hints.completeProductAndLocation',
        readyToCreate: 'inventory.inventoryForm.hints.readyToCreate',
        readyToUpdate: 'inventory.inventoryForm.hints.readyToUpdate',
      },
      validation: {
        searchRequired: 'inventory.inventoryForm.validation.searchRequired',
        invalidProductId: 'inventory.inventoryForm.validation.invalidProductId',
        productNotFound: 'inventory.inventoryForm.validation.productNotFound',
        productRequired: 'inventory.inventoryForm.validation.productRequired',
        locationRequired: 'inventory.inventoryForm.validation.locationRequired',
        stockLevelsNegative:
          'inventory.inventoryForm.validation.stockLevelsNegative',
        reorderGreaterThanMax:
          'inventory.inventoryForm.validation.reorderGreaterThanMax',
      },
      warnings: {
        reorderGreaterThanMax:
          'inventory.inventoryForm.warnings.reorderGreaterThanMax',
      },
      actions: {
        cancel: 'inventory.inventoryForm.actions.cancel',
        search: 'inventory.inventoryForm.actions.search',
        createInventory: 'inventory.inventoryForm.actions.createInventory',
        updateStockLevels:
          'inventory.inventoryForm.actions.updateStockLevels',
      },
      toasts: {
        searchErrorTitle: 'inventory.inventoryForm.toasts.searchErrorTitle',
        searchErrorMessage: 'inventory.inventoryForm.toasts.searchErrorMessage',
        invalidInputTitle: 'inventory.inventoryForm.toasts.invalidInputTitle',
        invalidInputMessage:
          'inventory.inventoryForm.toasts.invalidInputMessage',
        productFoundTitle: 'inventory.inventoryForm.toasts.productFoundTitle',
        productFoundMessage:
          'inventory.inventoryForm.toasts.productFoundMessage',
        productNotFoundTitle:
          'inventory.inventoryForm.toasts.productNotFoundTitle',
        productNotFoundMessage:
          'inventory.inventoryForm.toasts.productNotFoundMessage',
        searchFailedTitle: 'inventory.inventoryForm.toasts.searchFailedTitle',
        searchFailedMessage:
          'inventory.inventoryForm.toasts.searchFailedMessage',
        loadFailedTitle: 'inventory.inventoryForm.toasts.loadFailedTitle',
        loadFailedMessage: 'inventory.inventoryForm.toasts.loadFailedMessage',
        locationLoadErrorTitle:
          'inventory.inventoryForm.toasts.locationLoadErrorTitle',
        locationLoadErrorMessage:
          'inventory.inventoryForm.toasts.locationLoadErrorMessage',
        createSuccessTitle: 'inventory.inventoryForm.toasts.createSuccessTitle',
        createSuccessMessage:
          'inventory.inventoryForm.toasts.createSuccessMessage',
        createErrorTitle: 'inventory.inventoryForm.toasts.createErrorTitle',
        createErrorMessage: 'inventory.inventoryForm.toasts.createErrorMessage',
        updateSuccessTitle: 'inventory.inventoryForm.toasts.updateSuccessTitle',
        updateSuccessMessage:
          'inventory.inventoryForm.toasts.updateSuccessMessage',
        updateErrorTitle: 'inventory.inventoryForm.toasts.updateErrorTitle',
        updateErrorMessage: 'inventory.inventoryForm.toasts.updateErrorMessage',
      },
    },
    inventoryTable: {
      columns: {
        product: 'inventory.inventoryTable.columns.product',
        location: 'inventory.inventoryTable.columns.location',
        quantity: 'inventory.inventoryTable.columns.quantity',
        reorder: 'inventory.inventoryTable.columns.reorder',
        max: 'inventory.inventoryTable.columns.max',
        status: 'inventory.inventoryTable.columns.status',
        potentialProfit: 'inventory.inventoryTable.columns.potentialProfit',
      },
      dialogs: {
        deleteTitle: 'inventory.inventoryTable.dialogs.deleteTitle',
        deleteMessage: 'inventory.inventoryTable.dialogs.deleteMessage',
        deleteConfirmText: 'inventory.inventoryTable.dialogs.deleteConfirmText',
      },
      toasts: {
        deleteSuccessTitle: 'inventory.inventoryTable.toasts.deleteSuccessTitle',
        deleteSuccessMessage:
          'inventory.inventoryTable.toasts.deleteSuccessMessage',
        deleteErrorTitle: 'inventory.inventoryTable.toasts.deleteErrorTitle',
        deleteErrorMessage: 'inventory.inventoryTable.toasts.deleteErrorMessage',
      },
    },
    inventoryView: {
      title: 'inventory.inventoryView.title',
      subtitleId: 'inventory.inventoryView.subtitleId',
      tabs: {
        info: 'inventory.inventoryView.tabs.info',
        stock: 'inventory.inventoryView.tabs.stock',
        system: 'inventory.inventoryView.tabs.system',
      },
      sections: {
        productInformation: 'inventory.inventoryView.sections.productInformation',
        locationInformation:
          'inventory.inventoryView.sections.locationInformation',
        systemIdentifiers: 'inventory.inventoryView.sections.systemIdentifiers',
        auditTrail: 'inventory.inventoryView.sections.auditTrail',
        created: 'inventory.inventoryView.sections.created',
        updated: 'inventory.inventoryView.sections.updated',
      },
      fields: {
        productName: 'inventory.inventoryView.fields.productName',
        sku: 'inventory.inventoryView.fields.sku',
        category: 'inventory.inventoryView.fields.category',
        unitOfMeasure: 'inventory.inventoryView.fields.unitOfMeasure',
        name: 'inventory.inventoryView.fields.name',
        type: 'inventory.inventoryView.fields.type',
        address: 'inventory.inventoryView.fields.address',
        locationId: 'inventory.inventoryView.fields.locationId',
        quantityOnHand: 'inventory.inventoryView.fields.quantityOnHand',
        availableStock: 'inventory.inventoryView.fields.availableStock',
        reorderLevel: 'inventory.inventoryView.fields.reorderLevel',
        maxLevel: 'inventory.inventoryView.fields.maxLevel',
        stockLevel: 'inventory.inventoryView.fields.stockLevel',
        inventoryId: 'inventory.inventoryView.fields.inventoryId',
        productId: 'inventory.inventoryView.fields.productId',
        status: 'inventory.inventoryView.fields.status',
        createdByName: 'inventory.inventoryView.fields.createdByName',
        updatedByName: 'inventory.inventoryView.fields.updatedByName',
        userId: 'inventory.inventoryView.fields.userId',
        dateTime: 'inventory.inventoryView.fields.dateTime',
      },
      labels: {
        readyForUseOrSale: 'inventory.inventoryView.labels.readyForUseOrSale',
        minimumThreshold: 'inventory.inventoryView.labels.minimumThreshold',
        storageCapacity: 'inventory.inventoryView.labels.storageCapacity',
      },
      placeholders: {
        neverUpdated: 'inventory.inventoryView.placeholders.neverUpdated',
      },
    },
    locations: {
      buttonAdd: 'inventory.locations.buttonAdd',
      form: {
        title: {
          add: 'inventory.locations.form.title.add',
          edit: 'inventory.locations.form.title.edit',
        },
        sections: {
          locationInformation: 'inventory.locations.form.sections.locationInformation',
        },
        fields: {
          locationName: 'inventory.locations.form.fields.locationName',
          address: 'inventory.locations.form.fields.address',
          locationType: 'inventory.locations.form.fields.locationType',
          locationStatus: 'inventory.locations.form.fields.locationStatus',
        },
        placeholders: {
          locationName: 'inventory.locations.form.placeholders.locationName',
          address: 'inventory.locations.form.placeholders.address',
          selectLocationType:
            'inventory.locations.form.placeholders.selectLocationType',
        },
        hints: {
          locationActiveHint:
            'inventory.locations.form.hints.locationActiveHint',
          locationInactiveHint:
            'inventory.locations.form.hints.locationInactiveHint',
        },
        validation: {
          locationNameRequired:
            'inventory.locations.form.validation.locationNameRequired',
          addressRequired: 'inventory.locations.form.validation.addressRequired',
          locationTypeRequired:
            'inventory.locations.form.validation.locationTypeRequired',
        },
        actions: {
          cancel: 'inventory.locations.form.actions.cancel',
          create: 'inventory.locations.form.actions.create',
          saveChanges: 'inventory.locations.form.actions.saveChanges',
          creating: 'inventory.locations.form.actions.creating',
          saving: 'inventory.locations.form.actions.saving',
        },
        toasts: {
          loadTypesFailedTitle:
            'inventory.locations.form.toasts.loadTypesFailedTitle',
          loadTypesFailedMessage:
            'inventory.locations.form.toasts.loadTypesFailedMessage',
          fetchTypesFailedTitle:
            'inventory.locations.form.toasts.fetchTypesFailedTitle',
          fetchTypesFailedMessage:
            'inventory.locations.form.toasts.fetchTypesFailedMessage',
          createSuccessTitle: 'inventory.locations.form.toasts.createSuccessTitle',
          createSuccessMessage:
            'inventory.locations.form.toasts.createSuccessMessage',
          createFailedTitle: 'inventory.locations.form.toasts.createFailedTitle',
          createFailedMessage:
            'inventory.locations.form.toasts.createFailedMessage',
          updateSuccessTitle: 'inventory.locations.form.toasts.updateSuccessTitle',
          updateSuccessMessage:
            'inventory.locations.form.toasts.updateSuccessMessage',
          updateFailedTitle: 'inventory.locations.form.toasts.updateFailedTitle',
          updateFailedMessage:
            'inventory.locations.form.toasts.updateFailedMessage',
          loadLocationFailedTitle:
            'inventory.locations.form.toasts.loadLocationFailedTitle',
          loadLocationFailedMessage:
            'inventory.locations.form.toasts.loadLocationFailedMessage',
        },
      },
      table: {
        columns: {
          id: 'inventory.locations.table.columns.id',
          name: 'inventory.locations.table.columns.name',
          address: 'inventory.locations.table.columns.address',
          typeName: 'inventory.locations.table.columns.typeName',
          createdAt: 'inventory.locations.table.columns.createdAt',
          createdBy: 'inventory.locations.table.columns.createdBy',
        },
        dialogs: {
          deleteTitle: 'inventory.locations.table.dialogs.deleteTitle',
          deleteMessage: 'inventory.locations.table.dialogs.deleteMessage',
        },
        toasts: {
          deleteFailedTitle: 'inventory.locations.table.toasts.deleteFailedTitle',
          deleteFailedMessage:
            'inventory.locations.table.toasts.deleteFailedMessage',
          deleteSuccessTitle:
            'inventory.locations.table.toasts.deleteSuccessTitle',
          deleteSuccessMessage:
            'inventory.locations.table.toasts.deleteSuccessMessage',
        },
      },
      view: {
        title: 'inventory.locations.view.title',
        subtitleId: 'inventory.locations.view.subtitleId',
        loading: 'inventory.locations.view.loading',
        sections: {
          locationInformation: 'inventory.locations.view.sections.locationInformation',
          auditInformation: 'inventory.locations.view.sections.auditInformation',
          systemInformation: 'inventory.locations.view.sections.systemInformation',
          created: 'inventory.locations.view.sections.created',
          deleted: 'inventory.locations.view.sections.deleted',
        },
        fields: {
          locationName: 'inventory.locations.view.fields.locationName',
          address: 'inventory.locations.view.fields.address',
          locationType: 'inventory.locations.view.fields.locationType',
          locationTypeId: 'inventory.locations.view.fields.locationTypeId',
          dateTime: 'inventory.locations.view.fields.dateTime',
          user: 'inventory.locations.view.fields.user',
          userId: 'inventory.locations.view.fields.userId',
          locationId: 'inventory.locations.view.fields.locationId',
          status: 'inventory.locations.view.fields.status',
          isDeleted: 'inventory.locations.view.fields.isDeleted',
        },
        placeholders: {
          noAddressProvided: 'inventory.locations.view.placeholders.noAddressProvided',
          notDeleted: 'inventory.locations.view.placeholders.notDeleted',
        },
      },
    },
    stockTransfers: {
      buttonAdd: 'inventory.stockTransfers.buttonAdd',
      form: {
        title: {
          create: 'inventory.stockTransfers.form.title.create',
          view: 'inventory.stockTransfers.form.title.view',
        },
        tabs: {
          product: 'inventory.stockTransfers.form.tabs.product',
          transferDetails: 'inventory.stockTransfers.form.tabs.transferDetails',
          history: 'inventory.stockTransfers.form.tabs.history',
        },
        sections: {
          productInformation:
            'inventory.stockTransfers.form.sections.productInformation',
          selectedProduct: 'inventory.stockTransfers.form.sections.selectedProduct',
          transferRoute: 'inventory.stockTransfers.form.sections.transferRoute',
          transferSummary: 'inventory.stockTransfers.form.sections.transferSummary',
          transferTimeline: 'inventory.stockTransfers.form.sections.transferTimeline',
          currentStatus: 'inventory.stockTransfers.form.sections.currentStatus',
          transferInformation:
            'inventory.stockTransfers.form.sections.transferInformation',
          transferHistory: 'inventory.stockTransfers.form.sections.transferHistory',
          noHistoryAvailable:
            'inventory.stockTransfers.form.sections.noHistoryAvailable',
          transferCreated: 'inventory.stockTransfers.form.sections.transferCreated',
        },
        fields: {
          searchProduct: 'inventory.stockTransfers.form.fields.searchProduct',
          productName: 'inventory.stockTransfers.form.fields.productName',
          sku: 'inventory.stockTransfers.form.fields.sku',
          category: 'inventory.stockTransfers.form.fields.category',
          productId: 'inventory.stockTransfers.form.fields.productId',
          unitPrice: 'inventory.stockTransfers.form.fields.unitPrice',
          status: 'inventory.stockTransfers.form.fields.status',
          fromWarehouse: 'inventory.stockTransfers.form.fields.fromWarehouse',
          toWarehouse: 'inventory.stockTransfers.form.fields.toWarehouse',
          fromLocation: 'inventory.stockTransfers.form.fields.fromLocation',
          toLocation: 'inventory.stockTransfers.form.fields.toLocation',
          requestedQuantity:
            'inventory.stockTransfers.form.fields.requestedQuantity',
          transferNotes: 'inventory.stockTransfers.form.fields.transferNotes',
          totalValue: 'inventory.stockTransfers.form.fields.totalValue',
          transferId: 'inventory.stockTransfers.form.fields.transferId',
          createdBy: 'inventory.stockTransfers.form.fields.createdBy',
          userId: 'inventory.stockTransfers.form.fields.userId',
          createdDate: 'inventory.stockTransfers.form.fields.createdDate',
          dateTime: 'inventory.stockTransfers.form.fields.dateTime',
          user: 'inventory.stockTransfers.form.fields.user',
          fromLocationId: 'inventory.stockTransfers.form.fields.fromLocationId',
          toLocationId: 'inventory.stockTransfers.form.fields.toLocationId',
          createdByUserId:
            'inventory.stockTransfers.form.fields.createdByUserId',
        },
        placeholders: {
          searchProduct: 'inventory.stockTransfers.form.placeholders.searchProduct',
          searchPrompt: 'inventory.stockTransfers.form.placeholders.searchPrompt',
          selectWarehouse:
            'inventory.stockTransfers.form.placeholders.selectWarehouse',
          selectWarehouseFirst:
            'inventory.stockTransfers.form.placeholders.selectWarehouseFirst',
          notes: 'inventory.stockTransfers.form.placeholders.notes',
          noHistoryMessage:
            'inventory.stockTransfers.form.placeholders.noHistoryMessage',
          searchToStart: 'inventory.stockTransfers.form.placeholders.searchToStart',
          notSpecified: 'inventory.stockTransfers.form.placeholders.notSpecified',
          itemLabel: 'inventory.stockTransfers.form.placeholders.itemLabel',
        },
        labels: {
          singleProduct: 'inventory.stockTransfers.form.labels.singleProduct',
          multipleProducts:
            'inventory.stockTransfers.form.labels.multipleProducts',
          totalItems: 'inventory.stockTransfers.form.labels.totalItems',
          units: 'inventory.stockTransfers.form.labels.units',
          statusWithValue: 'inventory.stockTransfers.form.labels.statusWithValue',
        },
        validation: {
          selectProduct: 'inventory.stockTransfers.form.validation.selectProduct',
          selectFromLocation:
            'inventory.stockTransfers.form.validation.selectFromLocation',
          selectToLocation:
            'inventory.stockTransfers.form.validation.selectToLocation',
          locationsMustDiffer:
            'inventory.stockTransfers.form.validation.locationsMustDiffer',
          invalidQuantity: 'inventory.stockTransfers.form.validation.invalidQuantity',
          searchRequired: 'inventory.stockTransfers.form.validation.searchRequired',
          invalidProductId: 'inventory.stockTransfers.form.validation.invalidProductId',
        },
        actions: {
          cancel: 'inventory.stockTransfers.form.actions.cancel',
          createTransfer: 'inventory.stockTransfers.form.actions.createTransfer',
          changeStatus: 'inventory.stockTransfers.form.actions.changeStatus',
          ok: 'inventory.stockTransfers.form.actions.ok',
        },
        toasts: {
          transferCreateSuccessTitle:
            'inventory.stockTransfers.form.toasts.transferCreateSuccessTitle',
          transferCreateSuccessMessage:
            'inventory.stockTransfers.form.toasts.transferCreateSuccessMessage',
          transferCreateFailedTitle:
            'inventory.stockTransfers.form.toasts.transferCreateFailedTitle',
          transferCreateFailedMessage:
            'inventory.stockTransfers.form.toasts.transferCreateFailedMessage',
          productFoundTitle:
            'inventory.stockTransfers.form.toasts.productFoundTitle',
          productFoundMessage:
            'inventory.stockTransfers.form.toasts.productFoundMessage',
          productNotFoundTitle:
            'inventory.stockTransfers.form.toasts.productNotFoundTitle',
          productNotFoundMessage:
            'inventory.stockTransfers.form.toasts.productNotFoundMessage',
          searchFailedTitle: 'inventory.stockTransfers.form.toasts.searchFailedTitle',
          searchFailedMessage:
            'inventory.stockTransfers.form.toasts.searchFailedMessage',
          searchErrorTitle: 'inventory.stockTransfers.form.toasts.searchErrorTitle',
          searchErrorMessage:
            'inventory.stockTransfers.form.toasts.searchErrorMessage',
          invalidInputTitle: 'inventory.stockTransfers.form.toasts.invalidInputTitle',
          invalidInputMessage:
            'inventory.stockTransfers.form.toasts.invalidInputMessage',
          productSingleLocationError:
            'inventory.stockTransfers.form.toasts.productSingleLocationError',
          productNoInventoryError:
            'inventory.stockTransfers.form.toasts.productNoInventoryError',
        },
      },
      table: {
        columns: {
          product: 'inventory.stockTransfers.table.columns.product',
          fromLocation: 'inventory.stockTransfers.table.columns.fromLocation',
          toLocation: 'inventory.stockTransfers.table.columns.toLocation',
          quantity: 'inventory.stockTransfers.table.columns.quantity',
          status: 'inventory.stockTransfers.table.columns.status',
          user: 'inventory.stockTransfers.table.columns.user',
          createdAt: 'inventory.stockTransfers.table.columns.createdAt',
        },
      },
      view: {
        title: 'inventory.stockTransfers.view.title',
        transferCode: 'inventory.stockTransfers.view.transferCode',
        loading: 'inventory.stockTransfers.view.loading',
        tabs: {
          transferDetails: 'inventory.stockTransfers.view.tabs.transferDetails',
          items: 'inventory.stockTransfers.view.tabs.items',
          history: 'inventory.stockTransfers.view.tabs.history',
        },
        sections: {
          transferRoute: 'inventory.stockTransfers.view.sections.transferRoute',
          productInformation: 'inventory.stockTransfers.view.sections.productInformation',
          transferItem: 'inventory.stockTransfers.view.sections.transferItem',
          transferSummary: 'inventory.stockTransfers.view.sections.transferSummary',
          transferHistory: 'inventory.stockTransfers.view.sections.transferHistory',
          created: 'inventory.stockTransfers.view.sections.created',
          transferInformation:
            'inventory.stockTransfers.view.sections.transferInformation',
        },
        fields: {
          fromLocation: 'inventory.stockTransfers.view.fields.fromLocation',
          toLocation: 'inventory.stockTransfers.view.fields.toLocation',
          product: 'inventory.stockTransfers.view.fields.product',
          quantity: 'inventory.stockTransfers.view.fields.quantity',
          status: 'inventory.stockTransfers.view.fields.status',
          dateTime: 'inventory.stockTransfers.view.fields.dateTime',
          user: 'inventory.stockTransfers.view.fields.user',
          transferId: 'inventory.stockTransfers.view.fields.transferId',
          currentStatus: 'inventory.stockTransfers.view.fields.currentStatus',
          fromLocationId: 'inventory.stockTransfers.view.fields.fromLocationId',
          toLocationId: 'inventory.stockTransfers.view.fields.toLocationId',
          productId: 'inventory.stockTransfers.view.fields.productId',
          createdByUserId: 'inventory.stockTransfers.view.fields.createdByUserId',
        },
        placeholders: {
          fromLocation: 'inventory.stockTransfers.view.placeholders.fromLocation',
          toLocation: 'inventory.stockTransfers.view.placeholders.toLocation',
          notSpecified: 'inventory.stockTransfers.view.placeholders.notSpecified',
          itemLabel: 'inventory.stockTransfers.view.placeholders.itemLabel',
        },
        labels: {
          oneProduct: 'inventory.stockTransfers.view.labels.oneProduct',
          totalUnits: 'inventory.stockTransfers.view.labels.totalUnits',
          units: 'inventory.stockTransfers.view.labels.units',
          statusWithValue: 'inventory.stockTransfers.view.labels.statusWithValue',
        },
      },
    },
  },
};

export default i18nKeyContainer;
