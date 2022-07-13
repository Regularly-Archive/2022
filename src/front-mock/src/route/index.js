// export default router
import Vue from 'vue'
import VueRouter from 'vue-router'
import HelloWorld from '@/components/HelloWorld.vue'
import MessageList from '@/components/MessageList.vue'

Vue.use(VueRouter)

const route = new VueRouter({
    routes: [{
        path: '/',
        name: 'Home',
        component: HelloWorld,
    }, {
        path: '/home',
        name: 'Home',
        component: HelloWorld,
    }, {
        path: '/message',
        name: 'Message',
        component: MessageList,
    }]
})

export default route
