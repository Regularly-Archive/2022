<template>
  <div id="app">
    <img alt="Vue logo" src="./assets/logo.png" />
    <header>
      <my-router-link to="/home">首页</my-router-link>
      <span>/</span>
      <my-router-link to="/message">消息</my-router-link>
      <span>/</span>
      <my-router-link to="https://blog.yuanpei.me" target="_blank"
        >博客</my-router-link
      >
      <my-router-link
        :to="{ path: 'https://www.baidu.com/s', query: { wd: '天气' } }"
        target="_blank"
        >百度</my-router-link
      >
      <span>/</span>
      <my-router-link
        :to="{
          path: 'tencent://',
          query: { uin: '875974254', site: 'Vue', menu: 'yes' },
        }"
        >QQ</my-router-link
      >
    </header>
    <router-view />
  </div>
</template>

<script>
import MyRouterLink from "./components/MyRouterLink.vue";

export default {
  name: "App",
  components: {
    MyRouterLink,
  },
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
