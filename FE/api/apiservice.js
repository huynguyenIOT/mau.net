'use strict';
define(['app', 'components/factory/factory'], function (app) {

    app.service('PhongBanApiService', ['$http', 'constantsFactory', function ($http, constantsFactory) {

        var service = {};

        var basedUrl = constantsFactory.ApiUrl;

        service.GetAllPhongBan = function (curentPage, pageSize, textSearch) {
            var promise = $http({
                method: 'GET',
                url: basedUrl + constantsFactory.ApiPhongBanUrl + '?pageNumber=' + curentPage + '&pageSize=' + pageSize + '&textSearch=' + textSearch
            });
            return promise;
        };

        service.GetAllDonVi = function () {
            var promise = $http({
                method: 'GET',
                url: basedUrl + 'api/allorganizations'
            });
            return promise;
        };

        service.GetDonViById = function (id) {
            var promise = $http({
                method: 'GET',
                url: basedUrl + 'api/organizations/' + id
            });
            return promise;
        };


        service.GetAllDonViNotGroup = function () {
            var promise = $http({
                method: 'GET',
                url: basedUrl + 'api/organizations'
            });
            return promise;
        };

        service.GetAllPhongBanByParentId = function (parentId) {
            var promise = $http({
                method: 'GET',
                url: basedUrl + constantsFactory.ApiPhongBanUrl + '/Parent/' + parentId
            });
            return promise;
        };

        service.AddPhongBan = function (pData) {

            var promise = $http({
                method: 'POST',
                url: basedUrl + constantsFactory.ApiPhongBanUrl,
                headers: {
                    'Content-type': ' application/json'
                },
                data: pData
            });
            return promise;
        };

        service.EditPhongBan = function (pData) {

            var promise = $http({
                method: 'PUT',
                url: basedUrl + constantsFactory.ApiPhongBanUrl + '/' + pData.OrganizationID,
                headers: {
                    'Content-type': ' application/json'
                },
                data: pData
            });
            return promise;
        };

        service.DeletePhongBan = function (Id) {
            var promise = $http({
                method: 'DELETE',
                url: basedUrl + constantsFactory.ApiPhongBanUrl + '/' + Id,
                headers: {
                    'Content-type': ' application/json'
                }
            });

            return promise;
        };

        // Return service
        return service;

    }]);
});

