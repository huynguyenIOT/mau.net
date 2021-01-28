// 'use strict';
define(['jquery', 'app', 'components/factory/factory', 'components/service/apiservice', 'components/service/amdservice',
    'views/organizations/organizationsitem'], function (jQuery, app) {
        app.controller('PhongBanCtrl', ['$scope', '$filter', '$http', '$modal', 'constantsFactory', 'PhongBanApiService', '$timeout', '$log', '$location', 'constantsAMD',
        function ($scope, $filter, $http, $modal, constantsFactory, PhongBanApi, $timeout, $log, $location, constantsAMD) {

            /* Declare variables url form popup add,edit,info */
            var deleteDialogTemplateUrl = '/app-data/views/dialogtemp/deleteDialog.html';
            var itemDialogTemplateUrl = '/app-data/views/organizations/itemDialog.html';
            var infoDialogTemplateUrl = '/app-data/views/dialogtemp/infoDialog.html';
            // var openScenesDialogTemplateUrl = 'app-data/views/car/managerscenes.html';

            //LoadFormModule();


            /* Default visible button edit and delete*/
            $scope.visibleBtndel = false;
            $scope.visibleBtnedit = false;
            $scope.visibleBtnOpenF = false;

            /* Declare constant domain and path to Api */
            var apiUrl = constantsFactory.ApiUrl;
            var ApiPhongBanUrl = constantsFactory.ApiPhongBanUrl;

            /* Sorting default*/
            var orderBy = $filter('orderBy');
            $scope.order = function (predicate, reverse) {
                $scope.ListDataDefault = orderBy($scope.ListDataDefault, predicate, reverse);
            };
            $scope.order('-age', false);

            /* Header grid datatable */
            $scope.dataHeader = [
             { Key: 'STT', Value: "STT" },
             { Key: 'OrganizationName', Value: "Tên đơn vị/phòng ban" },
             { Key: 'ParentName', Value: "Tên đơn vị/phòng ban cha" },
             { Key: 'Loai', Value: "Loai" },
             { Key: 'SoDienThoai', Value: "Số điện thoại" },
             { Key: 'DiaChi', Value: "Địa chỉ" },
             { Key: 'Description', Value: "Mô tả" },
             { Key: 'CreateOndate', Value: "Ngày tạo" }
            ];

            /* Default variable to display in grid */
            $scope.curentPage = 1;
            $scope.pageSize = 10;
            $scope.recordCount = 0;
            $scope.pagging = "";
            $scope.CurentUrl = "";

            $scope.OrganizationList = {};


            /* Load data from API binding to Grid*/
            var loadData = function () {
                // Reset sorting class
                angular.forEach($scope.dataHeaderProgram, function (items) {
                    items.selected = null;
                });
                var filterText = "";
                if (typeof ($scope.filterText) !== 'undefined')
                    filterText = $scope.filterText;

                $scope.check = false;


                /* Disable previous, next */
                $scope.IsDisableNext = true;
                $scope.IsDisablePrevious = true;
                /* Default visible button edit and delete*/
                $scope.visibleBtndel = false;
                $scope.visibleBtnedit = false;
                $scope.visibleBtnOpenF = false;
                // Get data from api
                var promise = PhongBanApi.GetAllPhongBan($scope.curentPage, $scope.pageSize, filterText);
                promise.success(function (data) {
                    console.log(data);
                    if (data != null) {
                        //Enable previous button
                        if ($scope.curentPage > 1)
                            $scope.IsDisablePrevious = false;

                        //Load data to grid
                        $scope.ListDataDefault = data;

                        //Load record count
                        $scope.recordCount = data.length;
                        //Enable next button
                        if (data.length > $scope.pageSize) {
                            $scope.IsDisableNext = false;
                            $scope.ListDataDefault.splice($scope.pageSize, 1);
                            $scope.recordCount--;
                        }
                        //Show pagging
                        $scope.pagging = ($scope.curentPage - 1) * $scope.pageSize + 1 + " - " + (($scope.curentPage - 1) * $scope.pageSize + $scope.recordCount);
                    }
                }).error(function (response) {
                    $log.error();
                    //$log.debug(response);
                });
            };

            var urlRerite = $location.url();
            $scope.RootMenus = constantsAMD.LoadFormModule(urlRerite);
            loadData();


            /* Function button search */
            $scope.Search = function () {
                $scope.curentPage = 1;
                loadData();
            };

            /* Function button next */
            $scope.GoNext = function () {
                $scope.curentPage++;
                loadData();
            };

            /* Function button previous */
            $scope.GoPrev = function () {
                $scope.curentPage--;
                loadData();
            };

            /* This function click refesh grid program */
            $scope.refeshgrid = function () {
                $scope.curentPage = 1;
                $scope.filterText = "";
                loadData();
            };

            /* This function is to Check all item */
            $scope.CheckAllItem = function () {
                angular.forEach($scope.ListDataDefault, function (item) {
                    item.selected = $scope.check;
                });

                // If list has more than 1 item
                if ($scope.check && $scope.ListDataDefault.length > 1) {
                    $scope.visibleBtnedit = false;
                    $scope.visibleBtnOpenF = false;
                    $scope.visibleBtndel = true;
                }

                // If list has 1 item
                if ($scope.check && $scope.ListDataDefault.length == 1) {
                    $scope.visibleBtnedit = true;
                    $scope.visibleBtndel = true;
                    $scope.visibleBtnOpenF = true;
                }

                if (!$scope.check) {
                    $scope.visibleBtnedit = false;
                    $scope.visibleBtnOpenF = false;
                    $scope.visibleBtndel = false;
                }
            };

            $scope.GetHeaderItemCheckedClass = function (check) {
                if ($scope.check == true) {
                    return "checked";
                } else {
                    return "";
                };
            };

            $scope.GetItemCheckedClass = function (item) {
                if (item.selected == true) {
                    return "checked";
                } else {
                    return "";
                };
            };

            /* This function is to Check items in grid highlight*/
            $scope.GetItemClass = function (item) {
                if (item.selected == true) {
                    return 'selected';
                } else {
                    return '';
                };
            };
            /* This function is to Check items in grid highlight*/
            $scope.GetItemHeaderClass = function (item) {
                if (item.selected == true) {
                    return 'table-sort-asc';
                } else if (item.selected == false) {
                    return 'table-sort-desc';
                } else {
                    return 'table-sort-both';
                };
            };
            /* Sorting grid By Click to header */
            $scope.ClickToHeader = function (item) {
                // Set all class header to default
                angular.forEach($scope.dataHeaderProgram, function (items) {
                    if (items != item) {
                        items.selected = null;
                    };
                });
                // Set for item click sorted
                if (item.selected == true) {
                    item.selected = false;
                    $scope.order(item.Key, false);
                } else {
                    item.selected = true;
                    $scope.order(-item.Key, false);
                };
            };
            /* This function is to Check items in grid*/
            $scope.CheckItem = function (item) {
                var count = 0;
                angular.forEach($scope.ListDataDefault, function (item) {
                    if (item.selected == true) { count++; }
                });



                // Reset all buttons
                $scope.visibleBtnedit = false;
                $scope.visibleBtndel = false;
                $scope.visibleBtnOpenF = false;

                $scope.check = false;

                // If all items is checked
                if (count > 0 && count == $scope.ListDataDefault.length) {
                    $scope.visibleBtnedit = false;
                    $scope.visibleBtndel = true;
                    $scope.visibleBtnOpenF = false;
                    $scope.check = true;
                }

                if (count == 1) {
                    $scope.visibleBtnedit = true;
                    $scope.visibleBtndel = true;
                    $scope.visibleBtnOpenF = true;
                }

                if (count > 1) {
                    $scope.visibleBtnedit = false;
                    $scope.visibleBtndel = true;
                    $scope.visibleBtnOpenF = false;
                }

            };


            $scope.OpenAddForm = function (modalItem) {
                var modalInstance = $modal.open({
                    templateUrl: itemDialogTemplateUrl,
                    controller: 'PhongBanInstance',
                    size: 'md',
                    // Set parameter to chidform (popup form)
                    resolve: {
                        items: function () {
                            var obj = {};
                            obj.modalItem = modalItem;
                            return obj;
                        },
                        url: function () {
                            return $scope.CurentUrl;
                        },
                        options: function () {
                            return [{ Type: 'add' }];
                        }
                    }
                });
                modalInstance.result.then(function (newItem) {
                    $log.debug(newItem);
                    $scope.curentPage = 1;
                    loadData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });

            };

            /* This function click open form edit program */
            $scope.OpenEditForm = function (modalItem) {
                var count = 0;
                for (var i = 0; i < $scope.ListDataDefault.length; i++) {
                    if ($scope.ListDataDefault[i].selected == true) {
                        modalItem = $scope.ListDataDefault[i];
                        count++;
                    };
                };

                /* Code check to open form edit or info */
                if (count == 0) {
                    showModalDialog("Hãy chọn 1 chương trình", "Chú ý", 'Ok', 'Cancel');
                    return;
                };

                // Cannot open edit form when selected items count are more than 1
                if (count > 1) {
                    showModalDialog("Bạn chỉ được chọn 1 chương trình", "Chú ý", 'Ok', 'Cancel');
                };

                var modalInstance = $modal.open({
                    templateUrl: itemDialogTemplateUrl,
                    controller: 'PhongBanInstance',
                    size: 'md',
                    resolve: {
                        items: function () {
                            var obj = {};
                            obj.modalItem = modalItem;
                            return obj;
                        },
                        url: function () {
                            return $scope.CurentUrl;
                        },
                        options: function () {
                            return [{ Type: 'edit' }];
                        }
                    }
                });

                modalInstance.result.then(function (newItem) {

                    $scope.curentPage = 1;
                    $scope.check = false;
                    loadData();
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            };

            /* This function click open form Delete program */
            $scope.OpenDeleteForm = function (modalItem) {
                var listItemDelete = [];
                var count = 0;
                for (var i = 0; i < $scope.ListDataDefault.length; i++) {

                    if ($scope.ListDataDefault[i].selected == true) {
                        listItemDelete.push($scope.ListDataDefault[i]);
                        count++;
                    };

                };

                var message = "";
                if (count > 1) {
                    message = "Bạn có chắc chắn muốn xóa những đơnvị/phòng ban này !";
                } else {
                    message = "Bạn có chắc chắn muốn xóa đơn vị/phòng ban này !";
                };
                var infoResult = constantsAMD.OpenDialog(message, 'Chú ý', 'Đồng ý', 'Đóng', 'sm', null);

                infoResult.result.then(function (modalResult) {
                    if (modalResult == 'confirm') {
                        var listDeleteField = [];
                        var postData = {};
                        postData.ListItemDelete = listItemDelete;
                        $http({
                            method: 'POST',
                            url: apiUrl + ApiPhongBanUrl + '/deletes',
                            headers: {
                                'Content-type': ' application/json'
                            },
                            data: postData
                        })
                        .success(function (response) {
                            console.log(response.Data);
                            angular.forEach(response.Data, function (item) {
                                if (item.DeleteSuccess == true) {
                                    listDeleteField.push({ Name: item.OrganizationName + " - " + item.OrganizationID, Result: "Xóa thành công", Description: "" });
                                    //$log.debug(item);
                                } else if (item.DeleteSuccess == false) {
                                    //$log.debug(item);
                                    listDeleteField.push({ Name: item.OrganizationName + " - " + item.OrganizationID, Result: "Xóa thất bại", Description: "" });
                                };
                            });
                            $scope.curentPage = 1;
                            loadData();
                            var data = {};
                            data.Items = listDeleteField;
                            var infoDeleteResult = constantsAMD.OpenDialog('Kết quả xóa', 'Chú ý', '', 'Đóng', 'md', data);
                        });

                    };
                });
                /* Check all item is selected => Request to API  */
            };

        }]);

    });