//Call this to register our module to main application
var moduleName = "virtoCommerce.cacheModule";

if (AppDependencies != undefined) {
	AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.run(
  ['platformWebApp.toolbarService', '$window',
	function (toolbarService, $window) {

		// register reset store cache command
		var resetCacheCommand = {
			name: "reset cache",
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
					alert("Please set url or secure url for store");
				}
			},
			canExecuteMethod: function () { return true; },
			permission: 'cache:reset',
			index: 4
		};
		toolbarService.register(resetCacheCommand, 'virtoCommerce.storeModule.storeDetailController');
	}]);