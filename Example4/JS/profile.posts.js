// Профиль - добавление записи.
profilePostsApp = new Vue({
    el: '#profile-posts',
    data: {

        // Новые добавленые записи.
        contents: [],
        // Название записи.
        title: "",
        // Текст записи.
        text: "",
        // Видимость формы добавления записи.
        visibleAddPostForm: false,

        errorTitle: "",
        errorText: "",


        files: "",
        images: [],
        fileIndex: 0,





    },
    created: function () {
        //  this.getData();
        //    console.log('create');
    },

    methods: {
        handleFilesUpload() {
            this.fileIndex = 0;
            var vm = this;
            this.files = this.$refs.files.files;
            //console.log("handleFilesUpload", this.files);
            const reader = new FileReader;
            reader.onload = function (e) {
                vm.images.push(e.target.result);


                for (var i = vm.fileIndex; i < vm.files.length; i++) {
                    let file = vm.files[i];
                    vm.fileIndex++;
                    reader.readAsDataURL(file)
                    break;
                }
            }
            for (var i = this.fileIndex; i < this.files.length; i++) {
                let file = this.files[i];
                this.fileIndex++;
                reader.readAsDataURL(file)
                break;
            }



        },
        submitFile() {
            var vm = this;

            let formData = new FormData();
            for (var i = 0; i < this.files.length; i++) {
                let file = this.files[i];
                formData.append('files[' + i + ']', file);
            }
            formData.append('title', this.title);
            $.ajax({
                url: "/file/add",
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (data) {
                    //   alert(data);

                }
            });

        },

        // Добавить запись.
        submitPost: function () {
            // Сохранить данные.
            var vm = this;
            if (1 == 1) {
                let formData = new FormData();
                for (var i = 0; i < this.files.length; i++) {
                    let file = this.files[i];
                    formData.append('files[' + i + ']', file);
                }
                formData.append('title', this.title);
                formData.append('text', this.text);
                // formData.append('profileID', profileID);

                $.ajax({
                    url: "/content/add",
                    data: formData,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    success: function (response) {
                        if (response.status == "success") {
                            //  var content = response.content;
                            //content.date = response.date;
                            var content = {
                                date: response.date,
                                title: response.title,
                                text: response.text,
                                imageLink: response.imageLink
                            };
                            vm.contents.unshift(content);
                            //TODO сообщение вывести
                            //vm.visibleSuccessMessage = true;
                            //vm.errorAccess = false;
                            // Обновить список.
                            //vm.getData();
                            vm.visibleAddPostForm = false;
                        } else {
                            // Вывод ошибок
                            vm.errorText = response.errorText;
                            vm.errorTitle = response.errorTitle;
                            /*vm.errorAccess = response.errorAccess;*/
                        }
                    }
                });
            }


        },
        // Показать форму - Добавить запись.
        showAddPostForm: function () {
            // Сброс значений.            
            this.title = "";
            this.text = "";
            this.visibleAddPostForm = true;
            this.errorText = "";
            this.errorTitle = "";
            this.files = [];
            this.images = [];
        },

        // Скрыть форму - Добавить запись.
        hideAddPostForm: function () {
            this.visibleAddPostForm = false;
        },

        // Показать форму добавления комментария.
        showAddCommentForm: function (contentID) {
            console.log("showAddCommentForm", contentID);
            this.$eventBus.$emit('show-add-comment', contentID);
        },
        // 
        getCommentCountID: function (objectid) {
            return "comment-count-" + objectid;
        },


    }
});