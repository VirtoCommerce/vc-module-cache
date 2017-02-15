angular.module('virtoCommerce.cacheModule')
	.factory('virtoCommerce.cacheModule.changesTracking', ['$resource', function ($resource) {
	    return $resource('api/changes/force', {}, {
	        force: { method: 'POST' }
	    });
	}]);
 