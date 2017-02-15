//Call this to register our module to main application
var moduleName = "virtoCommerce.cacheModule";

if (AppDependencies != undefined) {
	AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.run(
  ['platformWebApp.toolbarService', '$window', '$translate', 'virtoCommerce.cacheModule.changesTracking',
	function (toolbarService, $window, $translate, changesTracking) {

		// register reset storefront cache command
		var resetCacheCommand = {
			name: "cache.reset-cache-command",
			icon: 'fa fa-eraser',
			executeMethod: function (blade) {
				var store = blade.currentEntity;	
			
				blade.isLoading = true;
					// {store_secure_url}/resetcache
				changesTracking.force({ scope: blade.currentEntity.id }, function (data) {
				    blade.isLoading = false;
				    $translate('cache.cache-reset-sucesfully').then(function (message) {
				        alert(message);
				    });
				});
			
			},
			canExecuteMethod: function () { return true; },
			permission: 'cache:reset',
			index: 4
		};
		toolbarService.register(resetCacheCommand, 'virtoCommerce.storeModule.storeDetailController');
	}]);