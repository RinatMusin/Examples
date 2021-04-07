Vue.component('select-city', {
    props: ['placeholder', 'cities'],
    template: `<div class="select-city-control">
        <div class="select-city-input" v-on:click="clickSelect()" >{{placeholder}}<i v-bind:class="checkIconCss()"></i></div>
        <div class="select-city-area" v-show="isOpen">
        <div class="select-city-reset"><span v-on:click="reset()" title="Сбросить фильтр">X</span></div>
            <div v-for="city in cities">
                <label><input type="checkbox" v-on:change="check()" v-model="city.value">{{city.name}}</input></label>
            </div>
        </div></div>`,
    data() {
        return {
            isOpen: false
        }
    },
    created: function () {
        //console.log(this.cities);
    },
    methods: {
        checkIconCss: function () {
            if (this.isOpen)
                return "open";
            else
                return "false";

        },
        clickSelect: function () {
            this.isOpen = !this.isOpen;

        },
        reset: function () {
            for (var i = 0; i < this.cities.length; i++) {
                this.cities[i].value = false;
            }
            this.check();
        },
        check: function () {
            var filterValue = "";
            for (var i = 0; i < this.cities.length; i++) {
                var element = this.cities[i];
                if (element.value) {
                    if (filterValue.length > 0)
                        filterValue += ",";
                    filterValue += element.id;
                }
            }
            // Формирование события
            this.$emit('change-city', filterValue);
        }
    }

});