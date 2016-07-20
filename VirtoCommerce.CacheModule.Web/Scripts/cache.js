//Call this to register our module to main application
var moduleName = "virtoCommerce.cacheModule";

if (AppDependencies != undefined) {
	AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.run(
  ['platformWebApp.toolbarService', '$window', '$translate',
	function (toolbarService, $window, $translate) {

		// register reset storefront cache command
		var resetCacheCommand = {
			name: "cache.reset-cache-command",
			icon: 'fa fa-eraser',
			executeMethod: function (blade) {
				var store = blade.currentEntity;
				if (store.url || store.secureUrl) {
					var url = store.secureUrl ? store.secureUrl : store.url;
					// {store_secure_url}/resetcache
					url += '/common/resetcache';
					$window.open(url, '_blank');
				}
				else
				{
					$translate('cache.store-url-empty-info').then(function (message) {
						alert(message);
					});
				}
			},
			canExecuteMethod: function () { return true; },
			permission: 'cache:reset',
			index: 4
		};
		toolbarService.register(resetCacheCommand, 'virtoCommerce.storeModule.storeDetailController');
	}]);