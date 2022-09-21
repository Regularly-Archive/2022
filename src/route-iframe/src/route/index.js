// export default router
import Vue from 'vue'
import VueRouter from 'vue-router'
import HelloWorld from '@/components/HelloWorld.vue'
import VueIFrame from '@/components/VueIFrame.vue'


Vue.use(VueRouter)

const route = new VueRouter({
    routes: [{
        path: '/home',
        name: 'Home',
        component: HelloWorld,
    }, {
        path: '/iframe',
        name: 'Iframe',
        component: VueIFrame,
    }]
})

export default route
