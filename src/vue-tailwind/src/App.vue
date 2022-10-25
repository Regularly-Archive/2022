<template>
  <div id="app">
    <hello-world/>
    <toast-form title="你是认真的吗" content="你是猴子请来的逗比吗" confirmText="Yes" cancelText="No"/>
  </div>
</template>

<script>
// import MyHeader from './components/Header.vue'
// import LoginForm from './components/LoginForm.vue'
import ToastForm from './components/ToastForm.vue'
import HelloWorld from './components/HelloWorld.vue'

export default {
  name: 'App',
  components: {
    // MyHeader,
    // LoginForm,
    ToastForm,
    HelloWorld
  }
}
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
