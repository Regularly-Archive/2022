<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <li>
      <ul :key="index" v-for="(message, index) in messageList">
        {{ message.eventTime }} - {{ message.content }}
      </ul>
    </li>
    <br/>
    <input type='button' @click="addMessage" value="Add Message"/>
  </div>
</template>

<script>
export default {
  name: "MessageList",
  props: {
    msg: String,
  },
  data() {
    return {
      messageList: [],
    };
  },
  mounted() {
      this.loadMessages()
  },

  methods: {
    loadMessages() {
      this.$http.get("/api/messages").then((res) => {
        let resData = res.data;
        console.log(resData)
        if (resData.code == 200) {
          this.messageList = resData.data;
        } else {
          this.messageList = [];
        }
      });
    },
    addMessage(){
      this.$http.post("/api/messages").then((res) => {
        let resData = res.data;
        if (resData.code == 200) {
          this.messageList = resData.data;
        } else {
          this.messageList = [];
        }
      });
    }
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
li {
  list-style: none;
  display: inline-block;
  margin: 0 10px;
}

ul {
  list-style-type: none;
  padding: 0;
  text-align: left;
}
</style>
